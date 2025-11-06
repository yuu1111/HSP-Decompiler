namespace KttK.HspDecompiler.Properties {
    
    
    // 縺薙・繧ｯ繝ｩ繧ｹ縺ｧ縺ｯ險ｭ螳壹け繝ｩ繧ｹ縺ｧ縺ｮ迚ｹ螳壹・繧､繝吶Φ繝医ｒ蜃ｦ逅・☆繧九％縺ｨ縺後〒縺阪∪縺・
    //  SettingChanging 繧､繝吶Φ繝医・縲∬ｨｭ螳壼､縺悟､画峩縺輔ｌ繧句燕縺ｫ逋ｺ逕溘＠縺ｾ縺吶・    //  PropertyChanged 繧､繝吶Φ繝医・縲∬ｨｭ螳壼､縺悟､画峩縺輔ｌ縺溷ｾ後↓逋ｺ逕溘＠縺ｾ縺吶・    //  SettingsLoaded 繧､繝吶Φ繝医・縲∬ｨｭ螳壼､縺瑚ｪｭ縺ｿ霎ｼ縺ｾ繧後◆蠕後↓逋ｺ逕溘＠縺ｾ縺吶・    //  SettingsSaving 繧､繝吶Φ繝医・縲∬ｨｭ螳壼､縺御ｿ晏ｭ倥＆繧後ｋ蜑阪↓逋ｺ逕溘＠縺ｾ縺吶・    internal sealed partial class Settings {
        
        internal Settings() {
            // // 險ｭ螳壹・菫晏ｭ倥→螟画峩縺ｮ繧､繝吶Φ繝・繝上Φ繝峨Λ繧定ｿｽ蜉縺吶ｋ縺ｫ縺ｯ縲∽ｻ･荳九・陦後・繧ｳ繝｡繝ｳ繝医ｒ隗｣髯､縺励∪縺・
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // SettingChangingEvent 繧､繝吶Φ繝医ｒ蜃ｦ逅・☆繧九さ繝ｼ繝峨ｒ縺薙％縺ｫ霑ｽ蜉縺励※縺上□縺輔＞縲・        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // SettingsSaving 繧､繝吶Φ繝医ｒ蜃ｦ逅・☆繧九さ繝ｼ繝峨ｒ縺薙％縺ｫ霑ｽ蜉縺励※縺上□縺輔＞縲・        }
    }
}
