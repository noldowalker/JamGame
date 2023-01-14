using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic;
using UnityEngine;
using UnityEngine.InputSystem;
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
        [SerializeField] 
        private float spheresRotationRadius = 30;
        [SerializeField] 
        private float spheresRotationSpeed = 50;
        
        private List<EnergySphere> _spheres = new List<EnergySphere>();

        private void Awake()
        {
            if (spheresAmount > 0)
                GeneratePoints(spheresAmount);

            SetShownPointsAmount(spheresAmount);
            IsRotationEnabled = true;
        }

        public void Update()
        {
            if (IsRotationEnabled)
                RotatePoints();
        }

        private void RotatePoints()
        {
            var center = transform.position;
            _spheres.ForEach(s =>
            {
                var newDegree = (s.lastDegree - spheresRotationSpeed * Time.deltaTime) % 360;
                var pos = DrawPointsByCircle(center, spheresRotationRadius, (int)newDegree);
                var sphereTransform = s.transform;
                sphereTransform.position = pos;
                s.lastDegree = newDegree;
            });
        }
        
        public void SetShownPointsAmount(int newAmount)
        {
            if (_spheres.Count < newAmount)
                GeneratePoints(newAmount);
                
            for (var i = 0; i < _spheres.Count; i++)
            {
                _spheres[i].gameObject.SetActive(i < newAmount);
            }
            
            IsRotationEnabled = _spheres.Count == newAmount;
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
                Vector3 pos = DrawPointsByCircle(center, spheresRotationRadius, nextPoint);
                var sphere = Instantiate(spherePrefab, transform);
                var sphereTransform = sphere.transform;
                sphereTransform.position = pos;
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
    }
}