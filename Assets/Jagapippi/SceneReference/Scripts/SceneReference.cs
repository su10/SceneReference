using System;
using System.IO;
using UnityEngine;

namespace Jagapippi.SceneReference
{
    [Serializable]
    public class SceneReference : ISceneReference
    {
        [SerializeField] private string _name = "";
        [SerializeField] private string _path = "";
        [SerializeField] private bool _enabled = false;
        [SerializeField] private int _buildIndex = -1;

        public string name => _name;
        public string path => _path;
        public bool enabled => _enabled;
        public int buildIndex => _buildIndex;

        public SceneReference()
        {
        }

        public SceneReference(string path, bool enabled = false, int buildIndex = -1)
            : this(Path.GetFileNameWithoutExtension(path), path, enabled, buildIndex)
        {
        }

        public SceneReference(string name, string path, bool enabled = false, int buildIndex = -1)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (enabled == false && buildIndex != -1) throw new ArgumentOutOfRangeException(nameof(buildIndex));

            _name = name;
            _path = path;
            _enabled = enabled;
            _buildIndex = buildIndex;
        }
    }
}