public interface Interactable
{
    string getDisplayText();
    void Interact();

}
public interface IUseAction
{
    void setActions();
    void unsetActions();
    bool ready { get; set; }
}