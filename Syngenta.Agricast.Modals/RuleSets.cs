﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=4.0.30319.1.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class rulesets {
    
    private rulesetsRuleset[] itemsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ruleset", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public rulesetsRuleset[] Items {
        get {
            return this.itemsField;
        }
        set {
            this.itemsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class rulesetsRuleset {
    
    private rulesetsRulesetRule[] ruleField;
    
    private string idField;
    
    private string daytimeindexField;
    
    private string startvalueField;
    
    private string valuepriorityField;
    
    private string basetrnidField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("rule", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public rulesetsRulesetRule[] rule {
        get {
            return this.ruleField;
        }
        set {
            this.ruleField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string daytimeindex {
        get {
            return this.daytimeindexField;
        }
        set {
            this.daytimeindexField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string startvalue {
        get {
            return this.startvalueField;
        }
        set {
            this.startvalueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string valuepriority {
        get {
            return this.valuepriorityField;
        }
        set {
            this.valuepriorityField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string basetrnid {
        get {
            return this.basetrnidField;
        }
        set {
            this.basetrnidField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class rulesetsRulesetRule {
    
    private rulesetsRulesetRuleCondition[] conditionField;
    
    private string valueField;
    
    private string letterField;
    
    private string adddescriptionField;
    
    private string hourfromField;
    
    private string hourtoField;
    
    private string idField;
    
    private string hourField;
    
    private string colorField;
    
    private string daytimeField;
    
    private string breakField;
    
    private string plidField;
    
    private string trnidField;
    
    private string imgidField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("condition", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public rulesetsRulesetRuleCondition[] condition {
        get {
            return this.conditionField;
        }
        set {
            this.conditionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string value {
        get {
            return this.valueField;
        }
        set {
            this.valueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string letter {
        get {
            return this.letterField;
        }
        set {
            this.letterField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string adddescription {
        get {
            return this.adddescriptionField;
        }
        set {
            this.adddescriptionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string hourfrom {
        get {
            return this.hourfromField;
        }
        set {
            this.hourfromField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string hourto {
        get {
            return this.hourtoField;
        }
        set {
            this.hourtoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string hour {
        get {
            return this.hourField;
        }
        set {
            this.hourField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string color {
        get {
            return this.colorField;
        }
        set {
            this.colorField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string daytime {
        get {
            return this.daytimeField;
        }
        set {
            this.daytimeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string @break {
        get {
            return this.breakField;
        }
        set {
            this.breakField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string plid {
        get {
            return this.plidField;
        }
        set {
            this.plidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string trnid {
        get {
            return this.trnidField;
        }
        set {
            this.trnidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string imgid {
        get {
            return this.imgidField;
        }
        set {
            this.imgidField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class rulesetsRulesetRuleCondition {
    
    private string startField;
    
    private string endField;
    
    private string decimateruleField;
    
    private string columnField;
    
    private string lowerField;
    
    private string upperField;
    
    private string interpolateruleField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string start {
        get {
            return this.startField;
        }
        set {
            this.startField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string end {
        get {
            return this.endField;
        }
        set {
            this.endField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string decimaterule {
        get {
            return this.decimateruleField;
        }
        set {
            this.decimateruleField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string column {
        get {
            return this.columnField;
        }
        set {
            this.columnField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string lower {
        get {
            return this.lowerField;
        }
        set {
            this.lowerField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string upper {
        get {
            return this.upperField;
        }
        set {
            this.upperField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string interpolaterule {
        get {
            return this.interpolateruleField;
        }
        set {
            this.interpolateruleField = value;
        }
    }
}
