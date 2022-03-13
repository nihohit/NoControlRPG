using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ExplosionScript : MonoBehaviour {
  private ParticleCallbackScript[] particles;
  private SpawnPool spawnPool;

  private int callbacksCalled;

  // Start is called before the first frame update
  void Awake() {
    spawnPool = FindObjectOfType<SpawnPool>();
    particles = GetComponentsInChildren<ParticleSystem>()
      .Select(system => system.gameObject.AddComponent<ParticleCallbackScript>())
      .ToArray();
    foreach (var particle in particles) {
      void callback(ParticleCallbackScript caller) {
        if (++callbacksCalled == particles.Length) {
          spawnPool.ExplosionDone(this);
        }
      }
      particle.Callback = callback;
    }
  }

  internal void StartExplosion() {
    callbacksCalled = 0;
    foreach (var particle in particles) {
      particle.RestartParticle();
    }
  }
}
