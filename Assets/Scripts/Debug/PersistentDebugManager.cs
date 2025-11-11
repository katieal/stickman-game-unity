using Items;
using SaveData;
using SettingsUI;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Items.Properties;
using UnityEngine.Events;
using System;
using System.Linq;
using Sirenix.Serialization;
using Combat;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
namespace Testing
{
    //[ExecuteInEditMode]
    public class PersistentDebugManager : SerializedMonoBehaviour
    {

        public Transform Point1; // mob
        public Transform Point2; // destination
        public Transform EnemyPoint; // enemy

        public float RadAngle;
        public float DegAngle; // facing angle, calculated in movelogic
        public float EnemyAngle;
        //public float Angle;

        public bool IsInLOS = false;

        //public CircleCollider2D _collider;


        //[Button]
        //public void PrintOverlap()
        //{
        //    List<Collider2D> results = new List<Collider2D>();

        //    ContactFilter2D filter = ContactFilter2D.noFilter;
        //    filter.layerMask = LayerMask.GetMask("HitBox");
        //    filter.useLayerMask = true;

        //    int num = _collider.Overlap(filter, results);

        //    Debug.Log($"num: {num}");
        //    foreach (Collider2D c in results)
        //    {
        //        Debug.Log(c.gameObject.name);
        //    }
        //}


        private void Update()
        {
            float deltax = Point1.position.x - Point2.position.x;
            float deltay = Point1.position.y - Point2.position.y;

            RadAngle = Mathf.Atan2(deltay, deltax);
            DegAngle = (RadAngle * Mathf.Rad2Deg);


            float rad = Mathf.Atan2(Point1.position.y - EnemyPoint.position.y, Point1.position.x - EnemyPoint.position.x);
            EnemyAngle = (rad * Mathf.Rad2Deg);

            IsInLOS = Mathf.Abs(Mathf.DeltaAngle(DegAngle, EnemyAngle)) <= 45;
            //Angle = Vector2.SignedAngle(new Vector2(deltay, deltax), Vector2.right);
        }

        private void OnDrawGizmos()
        {
            if (Point1 != null)
            {
                Quaternion rotation = Quaternion.AngleAxis(DegAngle, Vector3.forward);
                Vector2 direction = rotation * Vector2.left;

                Gizmos.color = Color.red;
                Gizmos.DrawRay(Point1.position, direction * 5f);

                Quaternion lrot = Quaternion.AngleAxis(DegAngle - 45, Vector3.forward);
                Vector2 ldir = lrot * Vector2.left;
                Gizmos.DrawRay(Point1.position, ldir * 5f);

                Quaternion rrot = Quaternion.AngleAxis(DegAngle + 45, Vector3.forward);
                Vector3 rdir = rrot * Vector2.left;
                Gizmos.DrawRay(Point1.position, rdir * 5f);
            }
        }
    }
}