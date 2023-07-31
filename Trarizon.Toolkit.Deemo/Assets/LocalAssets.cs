namespace Trarizon.Toolkit.Deemo.Assets;
public static class LocalAssets
{
    private static readonly string _rootFolder = "Assets";//Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
    private static readonly string _gameAssets = Path.Combine(_rootFolder, "GameAssets");

    public static string DeemoPianoNoteImageFile => Path.Combine(_gameAssets, "BlackNote.png");
    public static string DeemoNosoundNoteImageFile => Path.Combine(_gameAssets, "NosoundNote.png");
    public static string DeemoSlideNoteImageFile => Path.Combine(_gameAssets, "SlideNote.png");
}
