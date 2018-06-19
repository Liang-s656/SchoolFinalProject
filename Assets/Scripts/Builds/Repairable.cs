using UnityEngine;

public class Repairable : MonoBehaviour {
    private int maxHealth;
    public int health = 100;
    public int repairCooldown = 5;

    public GameObject effectDie;

    private void Start() {
        maxHealth = health;
    }
    public void AddHealth(int amount){
        health += amount;
        if(health < 0){
            if(effectDie != null) Instantiate(effectDie, transform.position, transform.rotation);
            Destroy(gameObject);
            //die
        }else if(health > maxHealth){
            health = maxHealth;
        }
    }

    public bool IsFullyRepaired(){
        return health == maxHealth;
    }

    public void DrawGUI(){
        Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
        GUI.Box(new Rect(pos.x + 20, Screen.height - pos.y - 42, 100, 20), "", "HpEmpty");
        GUI.Box(new Rect(pos.x + 20, Screen.height - pos.y - 42, 1 * health, 20), health + "Hp", "Hp");
    }
}
