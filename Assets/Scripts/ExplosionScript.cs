using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ExplosionScript : MonoBehaviour {
  private float duration;
  private ParticleSystem[] particles;
  private SpawnPool spawnPool;

  // Start is called before the first frame update
  void Awake() {
    particles = GetComponentsInChildren<ParticleSystem>();
    foreach (var particle in particles) {
      particle.Pause();
    }
    duration = particles.Max(particle => particle.main.duration);
    spawnPool = FindObjectOfType<SpawnPool>();
  }

  internal void StartExplosion() {
    StartCoroutine(ReportFinished());
    foreach (var particle in particles) {
      particle.Simulate(0, true, true);
      //   particle.Clear();
      //   particle.time = 0f;
      particle.Play();
    }
  }

  private IEnumerator ReportFinished() {
    yield return new WaitForSeconds(duration);
    spawnPool.ExplosionDone(this);
  }
}
