
        Blockly.Blocks['TransformRowMergeFields'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("TransformRowMergeFields");
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput('Merge fields'), 'fldName');
        
        this.appendValueInput('valNameField1') 
        .setCheck('String')
        .appendField('NameField1:')
        .appendField(new Blockly.FieldTextInput(''), 'fldNameField1');
        this.appendValueInput('valNameField2') 
        .setCheck('String')
        .appendField('NameField2:')
        .appendField(new Blockly.FieldTextInput(''), 'fldNameField2');
        this.appendValueInput('valNameFieldOutput') 
        .setCheck('String')
        .appendField('NameFieldOutput:')
        .appendField(new Blockly.FieldTextInput(''), 'fldNameFieldOutput');
        this.appendValueInput('valSeparator') 
        .setCheck('String')
        .appendField('Separator:')
        .appendField(new Blockly.FieldTextInput(''), 'fldSeparator');
        this.setTooltip("TransformRowMergeFields");
        this.setHelpUrl("");
        this.setOutput(true, "Transformer");
        }
        };
    