using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UserInterface.IngameInformation
{
    public class PointsCounter: MonoBehaviour
    {
        [SerializeField] 
        private Point pointSprite;
        [SerializeField] 
        private int pointsAmount;
        
        private List<Point> _points = new List<Point>();

        private void Awake()
        {
            if (pointsAmount > 0)
                GeneratePoints(pointsAmount);

            SetShownPointsAmount(0);
            ObserverWithoutData.Sub(Events.Button1Pressed, AddPoint);
        }

        public void SetShownPointsAmount(int newAmount)
        {
            for (var i = 0; i < _points.Count; i++)
            {
                Debug.Log(@$"SetShownPointsAmount {i} active = {i < newAmount}");
                _points[i].gameObject.SetActive(i < newAmount);
            }
        }
        
        public void GeneratePoints(int newPointsAmount)
        {
            if (_points.Any())
                _points.ForEach(Destroy);

            _points = new List<Point>();

            var center = transform.position;
            var startingAngle = 200;
            var step = 90 / newPointsAmount;
            for (int i = 0; i < newPointsAmount; i++)
            {
                var nextPoint = startingAngle - (i * step);
                Vector3 pos = DrawPointsByCircle(center, 1.0f, nextPoint);
                Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
                var point = Instantiate(pointSprite, pos, rot);
                var pointTransform = point.transform;
                pointTransform.SetParent(gameObject.transform);
                _points.Add(point);
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
            if (_currentPoints < _points.Count)
                _currentPoints++;
            else
                _currentPoints = 0;

            SetShownPointsAmount(_currentPoints);
        }
    }
}