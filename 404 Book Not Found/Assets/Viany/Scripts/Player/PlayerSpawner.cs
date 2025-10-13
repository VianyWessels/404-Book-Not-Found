using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private Transform modelHolder;
    [SerializeField] private GameObject[] characterPrefabs;

    private int currentIndex = -1;
    private GameObject currentModel;

    void Start()
    {
        UpdateCharacter(PlayerPrefs.GetInt("SelectedCharacter", 0));
    }

    void Update()
    {
        int savedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        if (savedIndex != currentIndex)
        {
            UpdateCharacter(savedIndex);
        }
    }

    public void UpdateCharacter(int index)
    {
        if (index < 0 || index >= characterPrefabs.Length) return;

        if (currentModel != null)
            Destroy(currentModel);

        currentModel = Instantiate(characterPrefabs[index], modelHolder);
        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.localRotation = Quaternion.identity;

        Animator animator = currentModel.GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("No Animator found in character prefab!");
            return;
        }

        var movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.animator = animator;

        var damage = GetComponent<PlayerDamage>();
        if (damage != null)
        {
            damage.animator = animator;

            Transform weaponTransform = currentModel.transform.Find("Weapon");
            if (weaponTransform != null)
            {
                damage.weapon = weaponTransform.gameObject;
                damage.weapon.SetActive(false);
            }
        }

        var health = GetComponent<PlayerHealth>();
        if (health != null) health.animator = animator;

        currentIndex = index;
    }
}
