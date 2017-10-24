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
	  x=transformArray(x);
	  //$("#fileGenerated").val(code);
      $("#fileGenerated").html(orderedStringify(x));
    }
 function transformArray(code){
	var valueFiltersAndTransformers=code["FiltersAndTransformers"];
	if('0' in valueFiltersAndTransformers){
		var value=valueFiltersAndTransformers['0'];
		if(Array.isArray(value))			
		{
			debugger;
			delete valueFiltersAndTransformers['0'];
			var nr=0;
			for(var i=0;i<value.length;i++){
				var valLoop=value[i];
				if(valLoop == null || valLoop=='null')
						continue;
				valueFiltersAndTransformers[nr++]=valLoop;
			}
			if(nr == 0)
				delete code["FiltersAndTransformers"]
		}
	}
	 
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