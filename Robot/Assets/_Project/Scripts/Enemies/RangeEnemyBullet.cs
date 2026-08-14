using System;
using Robot;
using UnityEngine;

public class RangeEnemyBullet : MonoBehaviour
{
   [Header("Configuracion")] 
   public float speed = 9f;
   public int damage = 5;
   public float lifeTime = 3f;
   
   [Header("Efectos")]
   public GameObject hitEffect;
   
   private Vector3 direction;
   private bool isReady;

   private void Start()
   {
      Destroy(gameObject, lifeTime);
   }

   private void Update()
   {
      if(!isReady) return;
      transform.position += direction * speed * Time.deltaTime;
   }

   public void SetDirection(Vector3 dir)
   {
      direction = dir.normalized;
      isReady = true;
      transform.right = direction;
   }

   private void OnCollisionEnter(Collision other)
   {
      if (other.gameObject.CompareTag("Player"))
      {
         PlayerStatsManager.Source.TakeDamage(damage);
      }

      if (hitEffect != null)
      {
         Instantiate(hitEffect, transform.position, Quaternion.identity);
      }
      
      Destroy(gameObject);
   }
}
