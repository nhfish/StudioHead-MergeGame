using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovieRecipePanel : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelRoot;
    public Transform requiredParent;
    public Transform optionalParent;
    public GameObject itemSlotPrefab;
    public Button submitButton;

    [Header("Talent Slots")]
    public Button writerButton;
    public Button directorButton;
    public Button actorButton;
    public TalentListPanel talentListPanel;

    private readonly List<MovieRecipeItemSlotUI> slotUIs = new();
    private TalentCard writer;
    private TalentCard director;
    private TalentCard actor;

    public Action<MovieRecipe> OnRecipeSubmitted;

    private void Awake()
    {
        if (submitButton != null)
            submitButton.onClick.AddListener(SubmitRecipe);
        if (writerButton != null)
            writerButton.onClick.AddListener(() => OpenTalentList(SetWriter));
        if (directorButton != null)
            directorButton.onClick.AddListener(() => OpenTalentList(SetDirector));
        if (actorButton != null)
            actorButton.onClick.AddListener(() => OpenTalentList(SetActor));
    }

    public void Open(MovieRecipeData data)
    {
        if (panelRoot != null)
            panelRoot.SetActive(true);

        ClearSlots();
        if (data == null)
            return;

        foreach (var dept in data.requiredDepartments)
            CreateSlot(dept, requiredParent);
        foreach (var dept in data.bonusDepartments)
            CreateSlot(dept, optionalParent);
    }

    public void Close()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    private void ClearSlots()
    {
        foreach (Transform child in requiredParent)
            Destroy(child.gameObject);
        foreach (Transform child in optionalParent)
            Destroy(child.gameObject);
        slotUIs.Clear();
    }

    private void CreateSlot(DepartmentType dept, Transform parent)
    {
        if (itemSlotPrefab == null || parent == null)
            return;
        var go = Instantiate(itemSlotPrefab, parent);
        var ui = go.GetComponent<MovieRecipeItemSlotUI>();
        if (ui != null)
            ui.SetDepartment(dept);
        slotUIs.Add(ui);
    }

    private void OpenTalentList(Action<TalentCard> callback)
    {
        if (talentListPanel == null)
            return;
        talentListPanel.PopulateTalentCards(card =>
        {
            callback?.Invoke(card);
            talentListPanel.gameObject.SetActive(false);
        });
        talentListPanel.gameObject.SetActive(true);
    }

    private void SetWriter(TalentCard card)
    {
        writer = card;
        UpdateTalentButtonImage(writerButton, card);
    }

    private void SetDirector(TalentCard card)
    {
        director = card;
        UpdateTalentButtonImage(directorButton, card);
    }

    private void SetActor(TalentCard card)
    {
        actor = card;
        UpdateTalentButtonImage(actorButton, card);
    }

    private void UpdateTalentButtonImage(Button btn, TalentCard card)
    {
        if (btn == null) return;
        var img = btn.GetComponent<Image>();
        if (img != null)
            img.sprite = card != null ? card.baseData.portrait : null;
    }

    /// <summary>
    /// Build a <see cref="MovieRecipe"/> from the current UI state.
    /// </summary>
    /// <returns>Fully populated recipe object.</returns>
    public MovieRecipe BuildRecipe()
    {
        MovieRecipe recipe = new MovieRecipe
        {
            writer = writer,
            director = director,
            actor = actor,
            submittedItems = new List<DepartmentItemData>()
        };

        foreach (var slot in slotUIs)
        {
            if (slot != null && slot.AssignedItem != null)
                recipe.submittedItems.Add(slot.AssignedItem);
        }

        return recipe;
    }

    private void SubmitRecipe()
    {
        var recipe = BuildRecipe();
        OnRecipeSubmitted?.Invoke(recipe);
        Close();
    }
}
