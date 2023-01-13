using System.Collections.Generic;
using System.Linq;
using GameLogic;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.HotkeysBar
{
    public class EnergySpheresContainer: MonoBehaviour
    {
        public bool IsRotationEnabled { get; set; } = false;

        [SerializeField] 
        private EnergySphere spherePrefab;
        [SerializeField] 
        private int spheresAmount;
        
        private List<EnergySphere> _spheres = new List<EnergySphere>();

        private void Awake()
        {
            if (spheresAmount > 0)
                GeneratePoints(spheresAmount);

            SetShownPointsAmount(spheresAmount);
            _currentPoints = spheresAmount;
            IsRotationEnabled = true;
            ObserverWithoutData.Sub(Events.Button1Pressed, AddPoint);
        }

        public void Update()
        {
            if (Input.GetKeyUp(KeyCode.U)) 
                ObserverWithoutData.FireEvent(Events.Button1Pressed);
            if (IsRotationEnabled)
                RotatePoints();
        }

        private void RotatePoints()
        {
            var center = transform.position;
            _spheres.ForEach(s =>
            {
                var newDegree = (s.lastDegree - 50 * Time.deltaTime) % 360;
                var pos = DrawPointsByCircle(center, 50f, (int)newDegree);
                //var rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
                var sphereTransform = s.transform;
                sphereTransform.position = pos;
                //sphereTransform.rotation = rot;
                s.lastDegree = newDegree;
            });
        }
        
        public void SetShownPointsAmount(int newAmount)
        {
            for (var i = 0; i < _spheres.Count; i++)
            {
                _spheres[i].gameObject.SetActive(i < newAmount);
            }
        }
        
        public void GeneratePoints(int newPointsAmount)
        {
            if (_spheres.Any())
                _spheres.ForEach(Destroy);

            _spheres = new List<EnergySphere>();

            var center = transform.position;
            var startingAngle = 200;
            var step = 360 / newPointsAmount;
            for (int i = 0; i < newPointsAmount; i++)
            {
                var nextPoint = startingAngle - (i * step);
                Vector3 pos = DrawPointsByCircle(center, 1.0f, nextPoint);
                //Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
                var sphere = Instantiate(spherePrefab, pos, transform.rotation);
                var sphereTransform = sphere.transform;
                sphere.lastDegree = nextPoint;
                
                sphereTransform.SetParent(gameObject.transform);
                _spheres.Add(sphere);
            }
        }
        
        private Vector3 DrawPointsByCircle (Vector3 center, float radius, float ang){
            Vector3 pos;
            pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            pos.z = center.z;
            return pos;
        }

        private int _currentPoints = 0;
        public void AddPoint()
        {
            if (_currentPoints < _spheres.Count)
                _currentPoints++;
            else
                _currentPoints = 0;

            IsRotationEnabled = _currentPoints == _spheres.Count;
            Debug.Log(@$"AddPoint {_currentPoints}");
            SetShownPointsAmount(_currentPoints);
        }
    }
}