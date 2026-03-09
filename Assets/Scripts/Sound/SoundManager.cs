using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource spawnSource;
    [SerializeField] private AudioSource directionChangeSource;
    [SerializeField] private AudioSource appearanceSource;
    [SerializeField] private AudioSource accelerationSource;
    [SerializeField] private AudioSource slowSource;
    [SerializeField] private AudioSource portalSource;
    [SerializeField] private AudioSource deathSource;
    [SerializeField] private AudioSource finishSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySpawn()
    {
        if (spawnSource != null && spawnSource.clip != null)
            spawnSource.PlayOneShot(spawnSource.clip);
    }

    public void PlayDirectionChange()
    {
        if (directionChangeSource != null && directionChangeSource.clip != null)
            directionChangeSource.Play();
    }

    public void PlayAppearance()
    {
        if (appearanceSource != null && appearanceSource.clip != null)
            appearanceSource.Play();
    }

    public void PlayAcceleration()
    {
        if (accelerationSource != null && accelerationSource.clip != null)
            accelerationSource.PlayOneShot(accelerationSource.clip);
    }

    public void PlaySlow()
    {
        if (slowSource != null && slowSource.clip != null)
            slowSource.PlayOneShot(slowSource.clip);
    }

    public void PlayPortal()
    {
        if (portalSource != null && portalSource.clip != null)
            portalSource.PlayOneShot(portalSource.clip);
    }

    public void PlayDeath()
    {
        if (deathSource != null && deathSource.clip != null)
            deathSource.PlayOneShot(deathSource.clip);
    }

    public void PlayFinish()
    {
        if (finishSource != null && finishSource.clip != null)
            finishSource.PlayOneShot(finishSource.clip);
    }
}