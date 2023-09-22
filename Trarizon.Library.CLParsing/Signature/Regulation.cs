using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Trarizon.Library.CLParsing.Attributes;
using Trarizon.Library.CLParsing.Exceptions;
using Trarizon.Library.CLParsing.Input;
using Trarizon.Library.CLParsing.Signature.CLParameters;
using Trarizon.Library.CLParsing.Signature.ParameterInfos;
using Trarizon.Library.CLParsing.Utility;

namespace Trarizon.Library.CLParsing.Signature;
internal sealed class Regulation
{
    private const BindingFlags AllInstanceMemberBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private static readonly ConcurrentDictionary<Type, Regulation> _cache = new();

    private ConstructorInfo? _constructorInfo;

    private readonly Dictionary<string, OptionParameter> _optionParametersByFullName;
    private readonly Dictionary<string, OptionParameter> _optionParametersByShortName;
    private readonly List<ValueParameter> _valueParameters;
    private IParameterInfo[] _constructorParameterInfos;

    public Type SourceType { get; }
    public IReadOnlyDictionary<string, OptionParameter> OptionParametersByFullName => _optionParametersByFullName;
    public IReadOnlyDictionary<string, OptionParameter> OptionParametersByShortName => _optionParametersByShortName;

    private Regulation(Type type)
    {
        SourceType = type;
        _optionParametersByFullName = new();
        _optionParametersByShortName = new();
        _valueParameters = new();

        // Initialize
        InitializeConstructorInfo();
        InitializeArguments();
    }

    #region Initialization

    private void InitializeConstructorInfo()
    {
        var allConstructorInfos = SourceType.GetConstructors(AllInstanceMemberBindingFlags);
        switch (allConstructorInfos.Length) {
            case 0: Throw.RegulationInitializeFailed($"No constructor defined in type <{SourceType.Name}>"); return;
            case 1: _constructorInfo = allConstructorInfos[0]; return;
            default:
                var constructorInfos = from c in allConstructorInfos
                                       where c.GetCustomAttribute<CLConstructorAttribute>() != null
                                       select c;
                if (constructorInfos.CountsMoreThan(1))
                    Throw.RegulationInitializeFailed($"Only one constructor can be marked {nameof(CLConstructorAttribute)}.");
                else {
                    // If only one ctor has [CLConstructor], use this ctor,
                    // If no ctor has [CLConstructor], set _constructorInfo null and the default constructor will called
                    _constructorInfo = constructorInfos.FirstOrDefault();
                }
                return;
        }
    }

    [MemberNotNull(nameof(_constructorParameterInfos))]
    private void InitializeArguments()
    {
        IEnumerable<IParameterInfo> fieldParamInfos =
            from f in SourceType.GetFields(AllInstanceMemberBindingFlags)
            where IsFieldValid(f)
            select new FieldParameterInfo(f);
        IEnumerable<IParameterInfo> propertyParamInfos =
            from p in SourceType.GetProperties(AllInstanceMemberBindingFlags)
            where IsPropertyValid(p)
            select new PropertyParameterInfo(p);
        foreach (IParameterInfo prm in fieldParamInfos.Concat(propertyParamInfos))
            TryAddParameter(prm);

        ParameterInfo[] constructorParamInfos = _constructorInfo?.GetParameters() ?? Array.Empty<ParameterInfo>();
        _constructorParameterInfos = new IParameterInfo[constructorParamInfos.Length];
        for (int i = 0; i < constructorParamInfos.Length; i++) {
            var cpi = new ConstructorParameterInfo(constructorParamInfos[i]);
            if (TryAddParameter(cpi))
                _constructorParameterInfos[i] = cpi;
        }

        _valueParameters.Sort((l, r) => Comparer<int>.Default.Compare(l.Attribute.Order, r.Attribute.Order));

        static bool IsFieldValid(FieldInfo field) => !field.IsInitOnly;
        static bool IsPropertyValid(PropertyInfo property) => property.CanWrite;
    }

    private bool TryAddParameter(IParameterInfo prmInfo)
    {
        IEnumerable<CLParameterAttribute> attrs = prmInfo.GetAttributes();

        // More than 1 [CLParameter]
        if (attrs.CountsMoreThan(1, out int count))
            return Throw.RegulationInitializeFailed<bool>($"Field/Property <{prmInfo.MemberName}> contains more than 1 {nameof(CLParameterAttribute)}.");
        // No [CLParameter]
        else if (count == 0)
            return false;

        switch (attrs.First()) {
            case CLOptionAttribute optAttr: AddOptionParameter(new OptionParameter(prmInfo, optAttr)); break;
            case CLValueAttribute valAttr: AddValueParameter(new ValueParameter(prmInfo, valAttr)); break;
            default: Throw.RegulationInitializeFailed("Unknown argument type"); break;
        }
        return true;

        void AddOptionParameter(OptionParameter option)
        {
            ValidateParameterName(option.Attribute.FullName);
            ValidateParameterName(option.Attribute.ShortName);

            if (!_optionParametersByFullName.TryAdd(option.Attribute.FullName, option))
                Throw.RegulationInitializeFailed($"Argument full name <{option.Attribute.FullName}> repeated.");
            if (option.Attribute.ShortName is not null
                && !_optionParametersByShortName.TryAdd(option.Attribute.ShortName, option)) {
                Throw.RegulationInitializeFailed($"Argument short name <{option.Attribute.ShortName}> repeated.");
            }
        }

        void AddValueParameter(ValueParameter value) => _valueParameters.Add(value);

        static void ValidateParameterName(string? parameterName)
        {
            if (parameterName == null)
                return;

            foreach (char c in parameterName) {
                if (char.IsWhiteSpace(c) || c == CLParser.PairSeperator)
                    Throw.RegulationInitializeFailed($"Parameter name <{parameterName}> invalid, parameter name cannot contains space or '='.");
            }
        }
    }

    #endregion

    public object Construct(RawArguments rawArguments)
    {
        CategorizedArguments args = new();
        foreach (var (name, prm) in _optionParametersByFullName) {
            if (rawArguments.GetOptionValue(name, out var rawValue))
                args.Add(prm.ParameterInfo, prm.ConvertValue(rawValue));
            else if (rawArguments.HasFlag(name))
                args.Add(prm.ParameterInfo, true.Box());
            else
                args.Add(prm.ParameterInfo, prm.Attribute.Default);
        }

        var values = rawArguments.GetValues();
        foreach (var param in _valueParameters) {
            if (values.IsEnd)
                args.Add(param.ParameterInfo, param.Attribute.Default);
            else
                args.Add(param.ParameterInfo, param.ConvertValue(ref values));
        }

        return CreateObject(args);
    }

    private object CreateObject(in CategorizedArguments args)
    {
        object obj;
        if (_constructorInfo == null)
            obj = Activator.CreateInstance(SourceType) ?? Throw.ArgumentsObjectCreateFailed<object>("Cannot create arguments instance");
        else {
            object?[] parameters = new object?[_constructorParameterInfos.Length];
            for (int i = 0; i < parameters.Length; i++) {
                IParameterInfo key = _constructorParameterInfos[i];
                parameters[i] = args.ConstructorParameters[key];
            }
            obj = _constructorInfo.Invoke(parameters);
        }

        foreach (var (prmInfo, val) in args.Properties) {
            prmInfo.SetValue(obj, val);
        }

        return obj;
    }

    public static Regulation Get(Type type)
        => _cache.GetOrAdd(type, t => new Regulation(t));

    public static Regulation Get<T>() => Get(typeof(T));

    private readonly ref struct CategorizedArguments
    {
        public readonly Dictionary<IParameterInfo, object?> ConstructorParameters = new();
        public readonly LinkedList<(IParameterPropertyInfo ParamInfo, object? Value)> Properties = new();

        public CategorizedArguments() { }

        public void Add(IParameterInfo parameterInfo, object? value)
        {
            if (parameterInfo is IParameterPropertyInfo propInfo)
                Properties.AddLast((propInfo, value));
            else
                ConstructorParameters.Add(parameterInfo, value);
        }
    }
}

