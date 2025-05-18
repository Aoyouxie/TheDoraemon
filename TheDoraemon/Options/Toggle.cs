
namespace TheDoraemon.Options
{
    public class CustomToggleOption : CustomOption
    {
        protected internal CustomToggleOption(int id, MultiMenu menu, string name, bool value = true) : base(id, menu, name,
            CustomOptionType.Toggle,
            value)
        {
            Format = val => (bool)val ? "<color=#CCFFFF>启用</color>" : "<color=#FF0000>禁用</color>";
        }

        protected internal bool Get()
        {
            return (bool)Value;
        }

        protected internal void Toggle()
        {
            Set(!Get());
        }

        public override void OptionCreated()
        {
            base.OptionCreated();
            var tgl = Setting.Cast<ToggleOption>();
            tgl.CheckMark.enabled = Get();
        }
    }
}