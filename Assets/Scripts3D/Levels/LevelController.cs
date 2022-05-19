using Assets.Level.Builders;
using UnityEngine;

namespace Assets.Scripts3D.Levels
{ 
    public class LevelController : MonoBehaviour
    {
        [SerializeField]
        Vector3Int _size;
        [SerializeField]
        int _roomCount;
        [SerializeField]
        Vector3Int _roomMaxSize;

        private Generator3D _levelGenerator;
        private LevelBuilder _levelBuilder;

        private void Start()
        {
            _levelGenerator = FindObjectOfType<Generator3D>();
            _levelBuilder = FindObjectOfType<LevelBuilder>();

            var level = _levelGenerator.Generate(_size, _roomCount, _roomMaxSize);
            _levelBuilder.Build(level);   
        }
    }
}