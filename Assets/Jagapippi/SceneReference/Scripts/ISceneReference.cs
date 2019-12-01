namespace Jagapippi.SceneReference
{
    public interface ISceneReference
    {
        string name { get; }
        string path { get; }
        bool enabled { get; }
        int buildIndex { get; }
    }
}