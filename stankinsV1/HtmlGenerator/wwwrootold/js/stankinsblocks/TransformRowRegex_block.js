
        Blockly.Blocks['TransformRowRegex'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("TransformRowRegex");
        
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput(''), 'fldName');
        this.appendValueInput('valRegexWithGroups') 
        .setCheck('String')
        .appendField('RegexWithGroups:')
        .appendField(new Blockly.FieldTextInput(''), 'fldRegexWithGroups');
        this.appendValueInput('valKey') 
        .setCheck('String')
        .appendField('Column Name:')
        .appendField(new Blockly.FieldTextInput(''), 'fldKey');
        
        this.setTooltip("TransformRowRegex");
        this.setHelpUrl("");
        this.setOutput(true, "Transformer");
        }
        };
    