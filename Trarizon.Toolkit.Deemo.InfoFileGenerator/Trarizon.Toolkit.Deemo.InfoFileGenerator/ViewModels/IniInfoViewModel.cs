using CommunityToolkit.Mvvm.ComponentModel;
using Trarizon.Toolkit.Deemo.InfoFileGenerator.Entities;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator.ViewModels;
internal sealed partial class IniInfoViewModel(ChartInfo info) : ObservableObject
{
    public string MusicName
    {
        get => info.MusicName;
        set => SetProperty(ref info.MusicName, value);
    }

    public string Composer
    {
        get => info.Composer;
        set => SetProperty(ref info.Composer, value);
    }

    public string Charter
    {
        get => info.Charter;
        set => SetProperty(ref info.Charter, value);
    }

    public string EasyLevel
    {
        get => info.Levels[0];
        set => SetProperty(ref info.Levels[0], value);
    }

    public string NormalLevel
    {
        get => info.Levels[1];
        set => SetProperty(ref info.Levels[1], value);
    }

    public string HardLevel
    {
        get => info.Levels[2];
        set => SetProperty(ref info.Levels[2], value);
    }

    public string ExtraLevel
    {
        get => info.Levels[3];
        set => SetProperty(ref info.Levels[3], value);
    }
}
