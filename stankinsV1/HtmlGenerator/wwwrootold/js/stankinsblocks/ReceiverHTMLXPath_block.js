
        Blockly.Blocks['ReceiverHTMLXPath'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("ReceiverHTMLXPath");
        
        this.appendValueInput('valXPaths') 
        .setCheck('Array')
        .appendField('XPaths:')
        .appendField(new Blockly.FieldTextInput(''), 'fldXPaths');
        this.appendValueInput('valAttributeNames') 
        .setCheck('Array')
        .appendField('AttributeNames:')
        .appendField(new Blockly.FieldTextInput(''), 'fldAttributeNames');
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput(''), 'fldName');
        this.appendValueInput('valFileToRead') 
        .setCheck('String')
        .appendField('FileToRead:')
        .appendField(new Blockly.FieldTextInput(''), 'fldFileToRead');
        
        this.setTooltip("ReceiverHTMLXPath");
        this.setHelpUrl("");
        this.setOutput(true, "Receiver");
        }
        };
    