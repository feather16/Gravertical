using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    // 重力
    [SerializeField] private float GRAVITY_SCALE;
    [SerializeField] private Sprite gravityArrow;
    [SerializeField] private Sprite gravityArrowNone;

    [SerializeField] private AudioClip selectSound;
    [SerializeField] private AudioClip backSound;
    [SerializeField] public AudioClip enemyDamageSound;
    [SerializeField] public AudioClip enemyDeathSound;

    private Button modeButton;

    public Vector2 gravity = new Vector2(0, 0);

    // ステージ
    public int stageMinX, stageMaxX, stageMinY, stageMaxY;

    public AudioSource audioSource;

    public float time { get; private set; } = 0;
    public int numTreasures { get; private set; } = 0;
    public int treasuresCollected = 0;
    public bool stageCleared { get; private set; } = false;

    public bool gameOver { get; private set; } = false;

    private bool escapePressedPrev = false;

    public static bool isPausing
    {
        get => Mathf.Approximately(Time.timeScale, 0);
        set => Time.timeScale = value ? 0 : 1;
    }

    public static bool menuWindowOpened
    {
        get
        {
            GameObject canvas = GameObject.Find("Canvas");
            GameObject panel = canvas.transform
                .Find("Menu")
                .Find("MenuPanel")
                .gameObject;
            return panel.activeSelf;
        }
        set
        {
            GameObject canvas = GameObject.Find("Canvas");
            GameObject panel = canvas.transform
                .Find("Menu")
                .Find("MenuPanel")
                .gameObject;
            panel.SetActive(value);
        }
    }

    public static bool howToControlWindowOpened
    {
        get
        {
            GameObject canvas = GameObject.Find("Canvas");
            GameObject panel = canvas.transform
                .Find("Menu")
                .Find("MenuPanel")
                .Find("HowToControlWindowPanel")
                .gameObject;
            return panel.activeSelf;
        }
        set
        {
            GameObject canvas = GameObject.Find("Canvas");
            GameObject panel = canvas.transform
                .Find("Menu")
                .Find("MenuPanel")
                .Find("HowToControlWindowPanel")
                .gameObject;
            panel.SetActive(value);
        }
    }

    public static bool toTitleWindowOpened
    {
        get
        {
            GameObject canvas = GameObject.Find("Canvas");
            GameObject panel = canvas.transform
                .Find("Menu")
                .Find("MenuPanel")
                .Find("ToTitleWindowPanel")
                .gameObject;
            return panel.activeSelf;
        }
        set
        {
            GameObject canvas = GameObject.Find("Canvas");
            GameObject panel = canvas.transform
                .Find("Menu")
                .Find("MenuPanel")
                .Find("ToTitleWindowPanel")
                .gameObject;
            panel.SetActive(value);
        }
    }

    public static bool clearWindowOpened
    {
        get
        {
            GameObject canvas = GameObject.Find("Canvas");
            GameObject panel = canvas.transform
                .Find("ClearPanel")
                .gameObject;
            return panel.activeSelf;
        }
        set
        {
            GameObject canvas = GameObject.Find("Canvas");
            GameObject panel = canvas.transform
                .Find("ClearPanel")
                .gameObject;
            panel.SetActive(value);
        }
    }

    public static bool gameOverWindowOpened
    {
        get
        {
            GameObject canvas = GameObject.Find("Canvas");
            GameObject panel = canvas.transform
                .Find("GameOverPanel")
                .gameObject;
            return panel.activeSelf;
        }
        set
        {
            GameObject canvas = GameObject.Find("Canvas");
            GameObject panel = canvas.transform
                .Find("GameOverPanel")
                .gameObject;
            panel.SetActive(value);
        }
    }

    public static Game Get()
    {
        return GameObject.Find("Main Camera").GetComponent<Game>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        SetWalls();
        audioSource = GetComponent<AudioSource>();

        numTreasures = CountTreasures();
        treasuresCollected = 0;

        // モードボタン
        GameObject statusPanel = GameObject.Find("StatusPanel");
        foreach (Transform transform in statusPanel.transform)
        {
            Button button;
            if (transform.TryGetComponent(out button))
            {
                if (button.name == "ModeButton") modeButton = button;
            }
        }

        // 操作方法テキスト
        TextAsset creditsFile = Resources.Load("Data/howto") as TextAsset;
        string creditsTextStr = creditsFile.text;
        GameObject canvas = GameObject.Find("Canvas");
        Text howtoText = canvas.transform
            .Find("Menu")
            .Find("MenuPanel")
            .Find("HowToControlWindowPanel")
            .Find("Text")
            .GetComponent<Text>();
        howtoText.text = creditsTextStr;
        var lines = creditsTextStr.Split('\n').Length;
        howtoText.fontSize = 450 / (lines + 1);
    }

    // Update is called once per frame
    private void Update()
    {
        Physics2D.gravity = GRAVITY_SCALE * gravity;
        time += Time.deltaTime;

        // 重力の矢印
        GameObject gravArrow = GameObject.Find("CurrentGravity");
        gravArrow.transform.eulerAngles
            = DirectionToRotation(gravity);
        gravArrow.GetComponent<Image>().sprite
            = gravity != Vector2.zero ? gravityArrow : gravityArrowNone;

        // プレイヤーをカメラで追う
        GameObject player = GameObject.Find(nameof(Player));
        if (player != null)
        {
            transform.position = new Vector3(
                player.transform.position.x,
                player.transform.position.y,
                transform.position.z
            );
        }

        // お宝の数を表示
        GameObject treasuresText = GameObject.Find("TreasuresText");
        Text text = treasuresText.GetComponent<Text>();
        text.text = $"{treasuresCollected} / {numTreasures}";

        // お宝全回収
        if (treasuresCollected == numTreasures)
        {
            if (!stageCleared)
            {
                OnStageClear();
            }
        }

        // ゲームオーバー
        if (GameObject.Find("Player") == null)
        {
            if (!gameOver)
            {
                OnGameOver();
            }
        }

        // エスケープボタン
        if (!escapePressedPrev && Input.GetKey(KeyCode.Escape))
        {
            OnMenuButtonPressed();
        }
        escapePressedPrev = Input.GetKey(KeyCode.Escape);
    }

    public static void OnMenuButtonPressed()
    {
        if (isPausing)
        {
            if (howToControlWindowOpened)
            {
                howToControlWindowOpened = false;
                Get().PlayBackSound();
            }
            else if (toTitleWindowOpened)
            {
                toTitleWindowOpened = false;
                Get().PlayBackSound();
            }
            else
            {
                Unpause();
            }
        }
        else
        {
            Pause();
        }
    }

    public static void OnStageClear()
    {
        isPausing = true;
        clearWindowOpened = true;
        Get().stageCleared = true;
    }

    public static void OnGameOver()
    {
        isPausing = true;
        gameOverWindowOpened = true;
        Get().gameOver = true;
    }

    public static void Pause()
    {
        isPausing = true;
        menuWindowOpened = true;
        howToControlWindowOpened = false;
        toTitleWindowOpened = false;
        Get().PlaySelectSound();
    }

    public static void Unpause()
    {
        isPausing = false;
        menuWindowOpened = false;
        howToControlWindowOpened = false;
        toTitleWindowOpened = false;
        Get().PlayBackSound();
    }

    public static void BackToTitle()
    {
        isPausing = false;
        Get().PlayBackSound();
        Get().Invoke(nameof(ChangeToTitleScene), 0.2f);
    }

    public static void RestartSameStage()
    {
        isPausing = false;
        Get().PlaySelectSound();
        Get().Invoke(nameof(ReloadGameScene), 0.2f);
    }

    public static void ToNextStage()
    {
        isPausing = false;
        Get().PlaySelectSound();
        Get().Invoke(nameof(LoadNextGameScene), 0.2f);
    }

    public void ChangeToTitleScene()
    {
        SceneManager.LoadScene("Scenes/Title", LoadSceneMode.Single);
    }
    public void ReloadGameScene()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name, 
            LoadSceneMode.Single
        );
    }

    public void LoadNextGameScene()
    {
        SceneManager.LoadScene(
            (GetCurrentStageNumber() + 1).ToString(),
            LoadSceneMode.Single
        );
    }

    public static int GetCurrentStageNumber()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        if (int.TryParse(activeSceneName, out int number))
        {
            return number;
        }
        else
        {
            return -1;
        }
    }

    public static int GetNumStages()
    {
        SortedSet<int> numbersSet = new SortedSet<int>();
        int index = 0;
        while (true)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(index);
            if (path.Length == 0)
            {
                break;
            }
            path = Path.GetFileNameWithoutExtension(path);
            if (int.TryParse(path, out int stageNumber))
            {
                numbersSet.Add(stageNumber);
            }

            index++;
        }
        List<int> numbersList = numbersSet.ToList();
        for (int i = 0; i < numbersList.Count; i++)
        {
            Debug.Assert(numbersList[i] == i + 1);
        }
        return numbersList.Count;
    }

    private void SetWalls()
    {
        // 壁を配置
        Entity[] entities = FindObjectsOfType<Entity>();
        float minX = entities[0].transform.position.x;
        float maxX = entities[0].transform.position.x;
        float minY = entities[0].transform.position.y;
        float maxY = entities[0].transform.position.y;
        foreach (Entity entity in entities)
        {
            Vector2 pos = entity.transform.position;
            Vector2 scl = entity.transform.localScale;
            minX = Mathf.Min(pos.x - (scl.x - 1) / 2 - 1, minX);
            maxX = Mathf.Max(pos.x + (scl.x - 1) / 2 + 1, maxX);
            minY = Mathf.Min(pos.y - (scl.y - 1) / 2 - 1, minY);
            maxY = Mathf.Max(pos.y + (scl.y - 1) / 2 + 1, maxY);
        }
        minX = Mathf.Floor(minX);
        maxX = Mathf.Ceil(maxX);
        minY = Mathf.Floor(minY);
        maxY = Mathf.Ceil(maxY);
        for (int x = (int)minX; x <= (int)maxX; x++)
        {
            for (int y = (int)minY; y <= (int)maxY; y++)
            {
                if(
                    x == (int)minX || x == (int)maxX || 
                    y == (int)minY || y == (int)maxY)
                {
                    string name = "Wall";
                    GameObject prefab = Resources.Load<GameObject>(@"Prefabs/" + name);
                    GameObject obj = Instantiate(
                        prefab, 
                        new Vector2(x, y), 
                        Quaternion.Euler(0, 0, 0)
                    );
                    obj.name = name;
                }
            }
        }
        stageMinX = (int)minX;
        stageMaxX = (int)maxX;
        stageMinY = (int)minY;
        stageMaxY = (int)maxY;
    }

    public void PlaySelectSound()
    {
        audioSource.PlayOneShot(selectSound, 0.4f);
    }

    public void PlayBackSound()
    {
        audioSource.PlayOneShot(backSound, 0.6f);
    }

    private int CountTreasures()
    {
        int count = 0;
        count += FindObjectsOfType<Jewel>().Length;
        count += FindObjectsOfType<Entity>()
            .Count(e => e.hasTreasure);
        return count;
    }

    public static Vector2 RotationToDirection(Vector3 rotation)
    {
        Vector2 ret;
        if (rotation.z == 0) ret = Vector2.up;
        else if (rotation.z == 90) ret = Vector2.left;
        else if (rotation.z == 180) ret = Vector2.down;
        else ret = Vector2.right;
        return ret;
    }

    public static Vector3 DirectionToRotation(Vector2 dir)
    {
        Vector3 ret;
        if (dir == Vector2.zero || dir == Vector2.up)
            ret = new Vector3(0, 0, 0);
        else if (dir == Vector2.left) ret = new Vector3(0, 0, 90);
        else if (dir == Vector2.down) ret = new Vector3(0, 0, 180);
        else ret = new Vector3(0, 0, 270);
        return ret;
    }
}
