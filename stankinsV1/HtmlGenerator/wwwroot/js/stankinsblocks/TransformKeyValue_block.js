
        Blockly.Blocks['TransformKeyValue'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("Transform2KeyValue");
        
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput('Transformer to key value'), 'fldName');
        
        this.setTooltip("TransformKeyValue");
        this.setHelpUrl("");
        this.setOutput(true, "Trasnsformer");
        }
        };
    