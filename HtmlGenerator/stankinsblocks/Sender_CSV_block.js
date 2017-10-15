
        Blockly.Blocks['Sender_CSV'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("Sender_CSV");
        
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput('Sender to CSV'), 'fldName');
       
        this.appendValueInput('valFileName') 
        .setCheck('String')
        .appendField('FileName:')
        .appendField(new Blockly.FieldTextInput(''), 'fldFileName');
		
        this.appendValueInput('valFileMode') 
        //.setCheck('String')
        .appendField('FileMode:')
        .appendField(new Blockly.FieldTextInput(''), 'fldFileMode');
        
        this.setTooltip("Sender_CSV");
        this.setHelpUrl("");
        this.setOutput(true, "Sender");
        }
        };
    