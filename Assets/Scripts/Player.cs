using System.Collections.Generic;

public class Player {
  public List<WeaponInstance> Weapons { set; get; }
  public List<WeaponInstance> AvailableItems { set; get; }

  public static readonly Player Instance = new() {
    Weapons = new List<WeaponInstance>{
      new WeaponInstance(WeaponConfig.MACHINE_GUN),
      new WeaponInstance(WeaponConfig.TWO_SHOT_SHOTGUN)
    },
    AvailableItems = new List<WeaponInstance>{
      new WeaponInstance(WeaponConfig.MISSILE),
      new WeaponInstance(WeaponConfig.FLAMER),
      new WeaponInstance(WeaponConfig.LASER),
      new WeaponInstance(WeaponConfig.RIFLE)
    }
  };
}