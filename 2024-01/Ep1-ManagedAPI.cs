// What I used to do...
var prefabEntity = SystemAPI.GetSingletonEntity<MobaPrefabs>();
var healthBarPrefab1 = state.EntityManager.GetComponentObject<UIPrefabs>(prefabEntity).HealthBar;

// Doesn't work...
var healthBarPrefab2 = SystemAPI.GetSingleton<UIPrefabs>().HealthBar;
var healthBarPrefab3 = SystemAPI.GetComponent<UIPrefabs>(prefabEntity).HealthBar;
var prefabEntity1 = SystemAPI.GetSingletonEntity<UIPrefabs>();

// What I do now...
var healthBarPrefab = SystemAPI.ManagedAPI.GetSingleton<UIPrefabs>().HealthBar;