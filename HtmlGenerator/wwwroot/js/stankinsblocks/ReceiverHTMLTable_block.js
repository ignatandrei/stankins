
        Blockly.Blocks['ReceiverHTMLTable'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("ReceiverHTMLTable");
        
        this.appendValueInput('valName') 
        //.setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput('Receiver HTML Table'), 'fldName');
        this.appendValueInput('valFileToRead') 
        .setCheck('String')
        .appendField('File name or url:')
        .appendField(new Blockly.FieldTextInput(''), 'fldFileToRead');
        
        this.setTooltip("ReceiverHTMLTable");
        this.setHelpUrl("");         
        this.setOutput(true, "Receiver");
        }
        };
    