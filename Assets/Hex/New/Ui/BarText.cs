using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class BarText : MonoBehaviour
{
    Camera cam;

    Color originBarColor;

    private bool isVisible;

    public bool IsVisible
    {
        get 
        {
            return isVisible;
        }

        private set
        {
            isVisible = value;

            gameObject.active = isVisible;

        }
    }
    [SerializeField] Image barBackground;
    [SerializeField] Image levelImg;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI popText;
    [SerializeField] TextMeshProUGUI productionText;
    [SerializeField] TextMeshProUGUI foodText;

    void Start()
    {
        cam = Camera.main;
        originBarColor = barBackground.color;

        ChangeText("");
    }

    void Update()
    {
        Rotate();
    }

    void Rotate()
    {
        if(CheckIfIsVisible())
        {
            Quaternion cameraRotation = cam.transform.rotation;

            // Z�sk�n� sm�ru od textu ke kame�e
            Vector3 direction = cam.transform.position - transform.position;

            // Sm�r pro oto�en� textu sm�rem ke kame�e
            Quaternion lookAtRotation = Quaternion.LookRotation(-direction.normalized);

            // Z�sk�n� rotace kamery na ose Y
            float cameraYRotation = cam.transform.eulerAngles.y;

            // Kombinace: zachov�n� Y rotace kamery a sm�rov�n� na kameru
            transform.rotation = cameraRotation;
        }
    }

    public void ChangeText(
    string name = null,
    string level = null,
    string population = null,
    string production = null,
    string food = null,
    float? levelProgress = null,
    Faction faction = null)
    {
        barBackground.color = faction != null ? faction.color : originBarColor;
        Debug.Log("Update");

        // Aktualizace jednotliv�ch textov�ch pol�
        nameText.text = !string.IsNullOrWhiteSpace(name) ? name : "";
        levelText.text = !string.IsNullOrWhiteSpace(level) ? level : "";
        popText.text = !string.IsNullOrWhiteSpace(population) ? population : "";
        productionText.text = !string.IsNullOrWhiteSpace(production) ? production : "";
        foodText.text = !string.IsNullOrWhiteSpace(food) ? food : "";

        // Aktualizace progress baru
        if (levelProgress.HasValue)
        {
            levelImg.fillAmount = levelProgress.Value;
            levelImg.color = levelProgress >= 0 ? Color.green : Color.red;
        }

        // Nastaven� viditelnosti na z�klad� p��tomnosti text�
        IsVisible = !string.IsNullOrWhiteSpace(name) ||
                    !string.IsNullOrWhiteSpace(level) ||
                    !string.IsNullOrWhiteSpace(population) ||
                    !string.IsNullOrWhiteSpace(production) ||
                    !string.IsNullOrWhiteSpace(food);
    }

    bool CheckIfIsVisible()
    {
        Vector3 viewPos = cam.WorldToViewportPoint(transform.position);
        bool view = viewPos.x > 0 && viewPos.x < 1 && viewPos.y > 0 && viewPos.y < 1 && viewPos.z > 0;
        if (!view) return false;
        if (!IsVisible) return false;
        return true;
    }
}
