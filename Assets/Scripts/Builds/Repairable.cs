using UnityEngine;

public class Repairable : MonoBehaviour {
    private int maxHealth;
    public int health = 100;
    public int repairCooldown = 5;

    private void Start() {
        maxHealth = health;
    }
    public void AddHealth(int amount){
        health += amount;
        if(health < 0){
            //die
        }else if(health > maxHealth){
            health = maxHealth;
        }
    }

    public bool IsFullyRepaired(){
        return health == maxHealth;
    }
}