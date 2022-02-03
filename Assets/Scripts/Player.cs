using System.Collections.Generic;

public class Player {
  public List<WeaponInstance> Weapons { set; get; }

  public static readonly Player Instance = new Player {
    Weapons = new List<WeaponInstance>{
      new WeaponInstance {
        config = WeaponConfig.LASER,
        timeToNextShot = 0
      },
      new WeaponInstance {
        config = WeaponConfig.MACHINE_GUN,
        timeToNextShot = 0
      },
      new WeaponInstance {
        config = WeaponConfig.MACHINE_GUN,
        timeToNextShot = 0
      }
    }
  };
}