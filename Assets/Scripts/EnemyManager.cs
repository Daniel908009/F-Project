using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyShip[] enemyShips;

    public static EnemyManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public EnemyShip[] GetEnemyShips()
    {
        return enemyShips;
    }
    public FloatingObject[] GetEnemyShipsFloatingObjects()
    {
        FloatingObject[] floatingObjects = new FloatingObject[enemyShips.Length];
        for (int i = 0; i < enemyShips.Length; i++)
        {
            floatingObjects[i] = enemyShips[i].GetComponent<FloatingObject>();
        }
        return floatingObjects;
    }
    public void RemoveEnemyShip(EnemyShip ship)
    {
        int index = System.Array.IndexOf(enemyShips, ship);
        if (index >= 0)
        {
            EnemyShip[] newArray = new EnemyShip[enemyShips.Length - 1];
            for (int i = 0, j = 0; i < enemyShips.Length; i++)
            {
                if (i != index)
                {
                    newArray[j++] = enemyShips[i];
                }
            }
            enemyShips = newArray;
        }
    }
}