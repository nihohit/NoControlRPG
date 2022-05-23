using System.Linq;
using UnityEngine;

public class ExplosionScript : MonoBehaviour {
  private ParticleCallbackScript[] particles;
  private SpawnPool spawnPool;

  private int callbacksCalled;

  // Start is called before the first frame update
  protected void Awake() {
    spawnPool = FindObjectOfType<SpawnPool>();
    particles = GetComponentsInChildren<ParticleSystem>()
      .Select(system => system.gameObject.AddComponent<ParticleCallbackScript>())
      .ToArray();
    foreach (var particle in particles) {
      void Callback(ParticleCallbackScript caller) {
        if (++callbacksCalled == particles.Length) {
          spawnPool.ExplosionDone(this);
        }
      }
      particle.Callback = Callback;
    }
  }

  internal void StartExplosion() {
    callbacksCalled = 0;
    foreach (var particle in particles) {
      particle.RestartParticle();
    }
  }
}
