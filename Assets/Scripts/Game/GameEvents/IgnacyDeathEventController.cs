public class IgnacyDeathEventController : SingleEventController
{
    private void OnEnable()
    {
        FindObjectOfType<FirstOilWellController>()?.KillIgnacy();
    }
}
