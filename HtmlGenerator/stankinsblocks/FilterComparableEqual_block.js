
        Blockly.Blocks['FilterComparableEqual'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("FilterComparableEqual");
        
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput('Equality'), 'fldName');
        this.appendValueInput('valFieldName') 
        .setCheck('String')
        .appendField('FieldName:')
        .appendField(new Blockly.FieldTextInput(''), 'fldFieldName');
        
		this.appendValueInput('valComparableType') 
        .setCheck('String')
        .appendField('ComparableType:')
        .appendField(new Blockly.FieldTextInput(''), 'fldComparableType');
        
		this.appendValueInput('valValue') 
        .setCheck('String')
        .appendField('Value:')
        .appendField(new Blockly.FieldTextInput(''), 'fldValue');
        this.appendValueInput('valHowToCompareValues') 
        //.setCheck('String')
        .appendField('HowToCompareValues:')
        .appendField(new Blockly.FieldTextInput(''), 'fldHowToCompareValues');
        
        this.setTooltip("FilterComparableEqual");
        this.setHelpUrl("");
        this.setOutput(true, "Filters");
        }
        };
    