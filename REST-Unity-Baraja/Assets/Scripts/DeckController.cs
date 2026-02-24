using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class DeckController : MonoBehaviour
{
    [Header("Clients")]
    public FakeApiClient fakeApi;
    public RickMortyClient rmApi;

    [Header("UI")]
    public TMP_Text projectOwnerText;   // tu nombre completo
    public TMP_Text userNameText;       // nombre del usuario actual
    public TMP_Dropdown userDropdown;   // cambia usuario

    [Header("Cards")]
    public Transform cardsParent;       // Content del ScrollView
    public CardItemView cardPrefab;     // prefab de carta

    void Start()
    {
        // Pon tu nombre completo aquí (y también en el texto del Canvas si quieres)
        if (projectOwnerText) projectOwnerText.text = "Jerónimo Montoya Giraldo";

        // Dropdown fijo: Usuario 1,2,3 (sin complicarte con lista desde API)
        if (userDropdown)
        {
            userDropdown.ClearOptions();
            userDropdown.AddOptions(new System.Collections.Generic.List<string> {
                "Usuario 1", "Usuario 2", "Usuario 3"
            });

            userDropdown.onValueChanged.AddListener(OnUserChanged);
        }

        // Cargar primer usuario
        OnUserChanged(0);
    }

    void OnUserChanged(int index)
    {
        int userId = index + 1; // 0->1, 1->2, 2->3
        StartCoroutine(LoadUserAndDeck(userId));
    }

    IEnumerator LoadUserAndDeck(int userId)
    {
        ClearCards();

        UserData user = null;

        if (fakeApi == null)
        {
            Debug.LogError("fakeApi no está asignado en DeckController.");
            yield break;
        }

        yield return fakeApi.GetUser(
            userId,
            onOk: (u) => user = u,
            onFail: (e) => Debug.LogError(e)
        );

        if (user == null)
        {
            Debug.LogError("No se pudo cargar el usuario.");
            yield break;
        }

        if (userNameText) userNameText.text = user.name;

        if (user.deck == null || user.deck.Length == 0)
        {
            Debug.LogWarning("El deck está vacío.");
            yield break;
        }

        if (rmApi == null)
        {
            Debug.LogError("rmApi no está asignado en DeckController.");
            yield break;
        }

        if (cardPrefab == null)
        {
            Debug.LogError("cardPrefab no está asignado en DeckController.");
            yield break;
        }

        if (cardsParent == null)
        {
            Debug.LogError("cardsParent (Content) no está asignado en DeckController.");
            yield break;
        }

        foreach (int cardId in user.deck)
        {
            RMCharacter ch = null;

            // 1) Traer datos del personaje (carta)
            yield return rmApi.GetCharacter(
                cardId,
                onOk: (c) => ch = c,
                onFail: (e) => Debug.LogError(e)
            );

            if (ch == null) continue;

            // 2) Instanciar carta y poner texto
            CardItemView item = Instantiate(cardPrefab, cardsParent);
            item.SetData(ch.name, ch.species);

            // 3) Traer imagen y asignar
            Texture2D tex = null;
            yield return rmApi.GetImage(
                ch.image,
                onOk: (t) => tex = t,
                onFail: (e) => Debug.LogError(e)
            );

            if (tex != null) item.SetImage(tex);
        }
    }

    void ClearCards()
    {
        if (cardsParent == null) return;

        for (int i = cardsParent.childCount - 1; i >= 0; i--)
            Destroy(cardsParent.GetChild(i).gameObject);
    }
}