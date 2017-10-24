  var workspace = Blockly.inject('blocklyDiv',
        {media: '../../media/',
         toolbox: document.getElementById('toolbox')});
    Blockly.Xml.domToWorkspace(document.getElementById('startBlocks'),
                               workspace);

    function showCode() {
      // Generate JavaScript code and display it.
      Blockly.JavaScript.INFINITE_LOOP_TRAP = null;
      var code = Blockly.JavaScript.workspaceToCode(workspace);
	  //debugger;
	  window.alert(code);
	  var x=eval(code);
	  x=transformReceiversSendersTransformer(x);
	  //$("#fileGenerated").val(code);
      $("#fileGenerated").html(orderedStringify(x));
    }
	function transformArray(code, theType){
	var valueType=code[theType];
	if('0' in valueType){
		var value=valueType['0'];
		if(Array.isArray(value))			
		{
			//debugger;
			delete valueType['0'];
			var nr=0;
			for(var i=0;i<value.length;i++){
				var valLoop=value[i];
				if(valLoop == null || valLoop=='null')
						continue;
				valueType[nr++]=valLoop;
			}
			if(nr == 0)
				delete code[theType]
		}
	}
		
	}
 function transformReceiversSendersTransformer(code){
	transformArray(code,"FiltersAndTransformers")
	transformArray(code,"Senders")
	transformArray(code,"Receivers")
	return code;
 }
    function runCode() {
      // Generate JavaScript code and run it.
      window.LoopTrap = 1000;
      Blockly.JavaScript.INFINITE_LOOP_TRAP =
          'if (--window.LoopTrap == 0) throw "Infinite loop.";\n';
      var code = Blockly.JavaScript.workspaceToCode(workspace);
      Blockly.JavaScript.INFINITE_LOOP_TRAP = null;
      try {
        eval(code);
      } catch (e) {
        alert(e);
      }
    }