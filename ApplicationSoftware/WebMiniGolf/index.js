if ( ! Detector.webgl ) Detector.addGetWebGLMessage();

// floor info
var board_width = 20;
var board_height = 60;
var board_data = []
// floor pins
var pins = []
var pin_width = 50;
// we use a cube for a pin
// the Y axis is the height = level / 2, with a level of pin_width/2

var container;
var camera, controls, scene, renderer;
var plane, cube;
var mouse, raycaster, isShiftDown = false;

var rollOverMesh, rollOverMaterial;
var cubeGeo, cubeMaterial;

// all pins are stored in objects array
// it should keep always the same except for a new map
// order does not matter
var objects = [];

// we store this to revert hover effect when the cursor left the previous one
var lastHoverObject;

// selected objects are shown in dark cyan
// order matters because we also store it's original height in case we need to reverse (e.g. preview changes in place for widgets)
var selectedObjects = [];
var lastSelectedObject;
var selectedObjectsHeight = [];

// for widgets, if centeredObject != null means there's a currently selected place to put a widget. We only store the center object becuase the widget can be different sizes when adjsuting
var centeredObject;
var centeredObjectHeight;

// original color
var pin_color =  0xffcf5e;
// hover color
var hover_color = 0x65ebf0;
// selected color
var selected_color = 0x0ebbc2;

var isMouseLeftDown = false;

// variables to sync with UI
var tool = 0;

// Select
var toolSelect = 1;
// Hole/Hill
var toolHoleSub = 0;
var toolHoleShape = 0;
var hole_width;
var hole_depth = 5;
var toolHoleRotation = 0;
var toolSlopeUp = 1;
var toolSlopeDouble = 0;
var toolSlopeShape = 0;
var toolSlopeRotation = 0;
var toolSlopeLength = 5;
var toolSlopeWidth = 5;
var toolSlopeHeight = 5;

var toolTextureRotation = 0;
var toolTextureType = 0;
var toolTextureLength = 5;
var toolTextureWidth = 5;
var toolTextureHeight = 5;
var toolTextureRepeat = 2;

// first time run threejs
init();
animate();

toolUpdate();

function run_threejs() {
  init();
  animate();
}

// update from UI, triggered by onchange/oninput in html
function toolUpdate() {

  if (document.getElementById('toolSelect').checked) {
    tool = 0;
  } else if (document.getElementById('toolHole').checked) {
    tool = 2;
  } else if (document.getElementById('toolSlope').checked) {
    tool = 3;
  } else if (document.getElementById('toolTexture').checked) {
    tool = 4;
  }


  // Tool Hole/Hill
  if (document.getElementById('toolHoleHole').checked) {
    toolHoleSub = 0;
  } else {
    toolHoleSub = 1;
  }

  if (document.getElementById('toolHoleHemisphere').checked) {
    toolHoleShape = 0;
  } else if (document.getElementById('toolHoleCone').checked) {
    toolHoleShape = 1;
  } else if (document.getElementById('toolHolePyramid').checked) {
    toolHoleShape = 2;
  //} else if (document.getElementById('toolHoleSpiral').checked) {
    //toolHoleShape = 3;
  }

  hole_width = document.getElementById('toolHoleWidth').value;
  document.getElementById('toolHoleWidthText').value = hole_width;

  hole_depth = document.getElementById('toolHoleDepth').value;
  document.getElementById('toolHoleDepthText').value = hole_depth;

  toolHoleRotation = document.getElementById('toolHoleRotate').value;
  document.getElementById('toolHoleRotateText').value = toolHoleRotation;
  toolHoleRotation = toolHoleRotation*Math.PI/180;

  // Tool Slope
  if (document.getElementById('toolSlopeUp').checked) {
    toolSlopeUp = 1;
  } else {
    toolSlopeUp = 0;
  }

  if (document.getElementById('toolSlopeDouble').checked) {
    toolSlopeDouble = 1;
  } else {
    toolSlopeDouble = 0;
  }

  if (document.getElementById('toolSlopeStraight').checked) {
    toolSlopeShape = 0;
  } else if (document.getElementById('toolSlopeCurve').checked) {
    toolSlopeShape = 1;
  }

  toolSlopeRotation = document.getElementById('toolSlopeRotate').value;
  document.getElementById('toolSlopeRotateText').value = toolSlopeRotation;
  toolSlopeRotation = toolSlopeRotation*Math.PI/180;

  toolSlopeWidth = document.getElementById('toolSlopeWidth').value;
  document.getElementById('toolSlopeWidthText').value = toolSlopeWidth;

  toolSlopeLength = document.getElementById('toolSlopeLength').value;
  document.getElementById('toolSlopeLengthText').value = toolSlopeLength;

  toolSlopeHeight = document.getElementById('toolSlopeHeight').value;
  document.getElementById('toolSlopeHeightText').value = toolSlopeHeight;

  // Tool Texture
  if (document.getElementById('toolTextureWave').checked) {
    toolTextureType = 0;
  } else if (document.getElementById('toolTextureRocky').checked) {
    toolTextureType = 1;
  }
  toolTextureWidth = document.getElementById('toolTextureWidth').value;
  document.getElementById('toolTextureWidthText').value = toolTextureWidth;

  toolTextureLength = document.getElementById('toolTextureLength').value;
  document.getElementById('toolTextureLengthText').value = toolTextureLength;

  toolTextureHeight = document.getElementById('toolTextureHeight').value;
  document.getElementById('toolTextureHeightText').value = toolTextureHeight;

  toolTextureRepeat = document.getElementById('toolTextureRepeat').value;
  document.getElementById('toolTextureRepeatText').value = toolTextureRepeat;

  toolTextureRotation = document.getElementById('toolTextureRotate').value;
  document.getElementById('toolTextureRotateText').value = toolTextureRotation;
  toolTextureRotation = toolTextureRotation*Math.PI/180;

  // we need this to have realtime widget preview when adjusting
  updateSelectedObjects();

}

// prevent from losing creation when switching tools
function toolUpdateSave() {
  deselectAll();
  if (centeredObject != null) {
    centeredObject.dispose;
    centeredObject = null;
  }
  toolUpdate();
}

function reset() {
  clear_board()
  board_width = document.getElementById("num_width").value;
  board_height = document.getElementById("num_height").value;
  board_data = []
  create_new_board_data()
  create_board();
}

// create a default board data
function create_new_board_data() {
  for (var i = 0; i < board_width; i+=1) {
    for (var j = 0; j < board_height; j+=1) {
      if (i == 0 || i == board_width - 1 || j == 0 || j == board_height -1 ) {
	pin = {col: i, row: j, step_val: 8}
      }
      else {
	pin = {col: i, row: j, step_val: 6}
      }
      board_data.push(pin);
    }
  }
}

function create_board() {
  for (var i = 0; i < board_data.length; i+=1) {
    cubeMaterial = new THREE.MeshLambertMaterial( { color: pin_color} );
    var pin = new THREE.Mesh( cubeGeo, cubeMaterial );
    pin.translateX(board_data[i].col*pin_width + pin_width/2);
    pin.translateZ(board_data[i].row*pin_width + pin_width/2);
    pinSetHeight(pin, board_data[i].step_val);
    objects.push(pin);
    scene.add(pin);
  }
}

function clear_board() {
  for (var i = 0; i < objects.length; i += 1) {
    scene.remove(objects[i]);
    objects[i].dispose;
  }
  objects = []
}

// deselect everything and revert to their original heights
function resetSelectedObjects() {
  if (selectedObjects != null) {
    for (var i = 0; i < selectedObjects.length; i += 1) {
      pinSetHeight(selectedObjects[i], selectedObjectsHeight[i]);
      selectedObjects[i].material.color.setHex( pin_color );
    }
    for (var i = 0; i < selectedObjects.length; i += 1) {
      selectedObjects[i].dispose;
    }
    for (var i = 0; i < selectedObjectsHeight.length; i += 1) {
      selectedObjectsHeight[i].dispose;
    }
    selectedObjects = []
    selectedObjectsHeight = []
  }

  render();
}

// deselect everything but keep any changes
function deselectAll() {
  if (selectedObjects != null) {
    for (var i = 0; i < selectedObjects.length; i += 1) {
      selectedObjects[i].material.color.setHex( pin_color );
    }
    for (var i = 0; i < selectedObjects.length; i += 1) {
      selectedObjects[i].dispose;
    }
    for (var i = 0; i < selectedObjectsHeight.length; i += 1) {
      selectedObjectsHeight[i].dispose;
    }
    selectedObjects = []
    selectedObjectsHeight = []
  }

  render();
}

function selectAll() {
  deselectAll();
  for (var i = 0; i < objects.length; i += 1) {
    selectedObjects.push(objects[i]);
    selectedObjectsHeight.push(objects[i].scale.y*2);
    objects[i].material.color.setHex( selected_color );
  }
  render();
}

function selectBorder() {
  deselectAll();
  for (var i = 0; i < objects.length; i += 1) {
    var col = (objects[i].position.x - pin_width/2) / pin_width;
    var row = (objects[i].position.z - pin_width/2) / pin_width;
    console.log(col);
    console.log(row);
    if ( col == 0 || col == board_width - 1 || row == 0 || row == board_height -1 ) {
      selectedObjects.push(objects[i]);
      selectedObjectsHeight.push(objects[i].scale.y*2);
      objects[i].material.color.setHex( selected_color );
    }
  }
  render();
}

function resetHover() {
  for (var i = 0; i < objects.length; i += 1) {
    if (objects[i].material.color.getHex() == hover_color) {
      objects[i].material.color.setHex( pin_color );
    }
  }
}

// Select: trigger when dragging height
function toolAdjustHeight() {
  var val = document.getElementById("toolSelectHeight").value;
  document.getElementById("toolSelectHeightText").value = val;
  if (selectedObjects != null) {
    for (var i = 0; i < selectedObjects.length; i += 1) {
      pinSetHeight(selectedObjects[i], val);
    }
  }
  render();
}

// Hole/Hill
function isInCircle(originPos, targetPos) {
  var r = pin_width*hole_width/2;
  var xSq = (targetPos.x - originPos.x) * (targetPos.x - originPos.x);
  var zSq = (targetPos.z - originPos.z) * (targetPos.z - originPos.z);
  var rSq = r * r;
  var del = rSq - xSq -zSq;
  var d = hole_depth*50/2;
  if (del > 0) {
    var h = Math.sqrt(d*d - xSq - zSq);
    if (h>0) {
      return h;
    }
  }
  return -1;
}

function isInCone(originPos, targetPos) {
  var r = pin_width*hole_width/2;
  var xSq = (targetPos.x - originPos.x) * (targetPos.x - originPos.x);
  var zSq = (targetPos.z - originPos.z) * (targetPos.z - originPos.z);
  var rSq = r * r;
  var del = rSq - xSq -zSq;
  if (del > 0) {
    return (r - Math.sqrt(xSq + zSq)) /r * hole_depth*50/2;
  }
  return -1;
}

function isInPyramid(originPos, targetPos) {

  // rotate back to straight and compare to the straight shape
  var targetPosRotated = rotate(targetPos.x, targetPos.z, originPos.x, originPos.z, toolHoleRotation);
  var targetPosRotatedX = targetPosRotated[0];
  var targetPosRotatedZ = targetPosRotated[1];

  var r = pin_width*hole_width/2;
  if ( targetPosRotatedX < originPos.x + r && targetPosRotatedX > originPos.x - r && targetPosRotatedZ < originPos.z + r && targetPosRotatedZ > originPos.z -r ) {
    var dx = Math.abs(targetPosRotatedX - originPos.x);
    var dz = Math.abs(targetPosRotatedZ - originPos.z);
    if (dx > dz) {
      return (r - dx)/r * hole_depth*50/2
    } else {
      return (r - dz)/r * hole_depth*50/2
    }
  }

  //var r = pin_width*hole_width/2;
  //if ( targetPos.x < originPos.x + r && targetPos.x > originPos.x - r && targetPos.z < originPos.z + r && targetPos.z > originPos.z -r ) {
    //var dx = Math.abs(targetPos.x - originPos.x);
    //var dz = Math.abs(targetPos.z - originPos.z);
    //if (dx > dz) {
      //return (r - dx)/r * hole_depth*50/2
    //} else {
      //return (r - dz)/r * hole_depth*50/2
    //}
  //}
  return -1;
}


function isInSpiral(originPos, targetPos) {
  // TODO
  //var xSq = (targetPos.x - originPos.x) * (targetPos.x - originPos.x);
  //var x = targetPos.x - originPos.x;
  //var zSq = (targetPos.z - originPos.z) * (targetPos.z - originPos.z);
  //var z = targetPos.z - originPos.z;
  //var a = 4*pin_width;
  //var sSq =  a*a*Math.atan2(z, x)*Math.atan2(z, x);

  //if ( Math.abs(sSq - xSq - zSq) < pin_width*pin_width*16) {
    //return 2;
  //}

  return -1;
}

// rotate z,y along origin ox, oy with angle a
function rotate(x,y,ox,oy,a) { 
  return [(x-ox)*Math.cos(a) - (y-oy)*Math.sin(a) + ox,
             (x-ox)*Math.sin(a) + (y-oy)*Math.cos(a) + oy];
}

function isInSlope(originPos, targetPos) {
  // rotate back to straight and compare to the straight shape
  var targetPosRotated = rotate(targetPos.x, targetPos.z, originPos.x, originPos.z, toolSlopeRotation);
  var targetPosRotatedX = targetPosRotated[0];
  var targetPosRotatedZ = targetPosRotated[1];
  var w = toolSlopeWidth*pin_width;
  var l = toolSlopeLength*pin_width;
  var h = toolSlopeHeight*pin_width/2;
  if (targetPosRotatedX >= originPos.x && targetPosRotatedX <= originPos.x + l && targetPosRotatedZ >= originPos.z - w/2 && targetPosRotatedZ <= originPos.z + w/2) {
    if (toolSlopeShape == 0) {
      return (l - (targetPosRotatedX - originPos.x)) / l * h;
    } else if (toolSlopeShape == 1) {
      return (l - (targetPosRotatedX - originPos.x)) * (l - (targetPosRotatedX - originPos.x)) / l / l * h;
    }
  }
  if (toolSlopeDouble) {
    if (targetPosRotatedX <= originPos.x && targetPosRotatedX >= originPos.x - l && targetPosRotatedZ <= originPos.z + w/2 && targetPosRotatedZ >= originPos.z - w/2) {
      if (toolSlopeShape == 0) {
	return (l + (targetPosRotatedX - originPos.x)) / l * h;
      } else if (toolSlopeShape == 1) {
	return (l + (targetPosRotatedX - originPos.x)) * (l + (targetPosRotatedX - originPos.x)) / l / l * h;
      }
    }
  }
  return -1;
}

function isInRect(originPos, targetPos) {
  // rotate back to straight and compare to the straight shape
  var targetPosRotated = rotate(targetPos.x, targetPos.z, originPos.x, originPos.z, toolTextureRotation);
  var targetPosRotatedX = targetPosRotated[0];
  var targetPosRotatedZ = targetPosRotated[1];
  var w = toolTextureWidth*pin_width;
  var l = toolTextureLength*pin_width;
  var h = toolTextureHeight*pin_width/2;
  var repeat = toolTextureRepeat;

  if (targetPosRotatedX >= originPos.x - l/2 && targetPosRotatedX <= originPos.x + l/2 && targetPosRotatedZ >= originPos.z - w/2 && targetPosRotatedZ <= originPos.z + w/2) {
    if (toolTextureType == 0) {
      // wave: sin wave on X axis
      return h * Math.sin((targetPosRotatedX - (originPos.x - l/2)) / l * repeat * 2 * Math.PI);
    } else if (toolTextureType == 1) {
      // 2d wave: sine wave on X and Z axis
      var sinX = Math.sin((targetPosRotatedX - (originPos.x - w/2)) / l * repeat * 2 * Math.PI);
      var sinZ = Math.sin((targetPosRotatedZ - (originPos.z - w/2)) / l * repeat * 2 * Math.PI);
      return h * sinX * sinZ;
    }
    return 10;

  }


  return -1;
}

function updateSelectedObjects() {
  if (selectedObjects != null && centeredObject != null) {
    resetSelectedObjects();
    if (tool == 2) {
      updateHolePreview(centeredObject, selected_color);
    } else if (tool == 3) {
      updateSlopePreview(centeredObject, selected_color);
    } else if (tool == 4) {
      updateTexturePreview(centeredObject, selected_color);
    }
  }
  render();
}

function updateHolePreview(center, color) {
  for (var i = 0; i < objects.length; i += 1) {
    var check = -1;
    if (toolHoleShape == 0) {
      check = isInCircle(center.position, objects[i].position);
    } else if (toolHoleShape == 1) {
      check = isInCone(center.position, objects[i].position);
    } else if (toolHoleShape == 2) {
      check = isInPyramid(center.position, objects[i].position);
    } else if (toolHoleShape == 3) {
      check = isInSpiral(center.position, objects[i].position);
    }
    if (check != -1){
      selectedObjects.push(objects[i]);
      selectedObjectsHeight.push(objects[i].scale.y*2);
      objects[i].material.color.setHex( color );
      var change = Math.floor(check/50*2);
      if (toolHoleSub == 1) {
	// hill
	pinSetHeight(objects[i], centeredObjectHeight + change);
      } else {
	// hole
	pinSetHeight(objects[i], centeredObjectHeight - change);
      }
    }
  }
}

function updateSlopePreview(center, color) {
  for (var i = 0; i < objects.length; i += 1) {
    var check = -1;
    check = isInSlope(center.position, objects[i].position);
    if (check != -1){
      selectedObjects.push(objects[i]);
      selectedObjectsHeight.push(objects[i].scale.y*2);
      objects[i].material.color.setHex( color );
      var change = Math.floor(check/50*2);
      if (toolSlopeUp == 1) {
	// hill
	pinSetHeight(objects[i], centeredObjectHeight + change);
      } else {
	// hole
	pinSetHeight(objects[i], centeredObjectHeight - change);
      }
    }
  }
}

function updateTexturePreview(center, color) {
  for (var i = 0; i < objects.length; i += 1) {
    var check = -1;
    check = isInRect(center.position, objects[i].position);
    if (check != -1){
      selectedObjects.push(objects[i]);
      selectedObjectsHeight.push(objects[i].scale.y*2);
      objects[i].material.color.setHex( color );
      var change = Math.floor(check/50*2);
      if (toolSlopeUp == 1) {
	// hill
	pinSetHeight(objects[i], centeredObjectHeight + change);
      } else {
	// hole
	pinSetHeight(objects[i], centeredObjectHeight - change);
      }
    }
  }
}

function isInArea(startPos, endPos, targetPos) {
  var x0 = startPos.x;
  var z0 = startPos.z;
  var x1 = endPos.x;
  var z1 = endPos.z;
  var x = targetPos.x;
  var z = targetPos.z;

  if (x1 > x0) {
    if (z1 > z0) {
      if (x >= x0 && x <= x1 && z >= z0 && z <= z1) {
	return true;
      }
    } else {
      if (x >= x0 && x <= x1 && z <= z0 && z >= z1) {
	return true;
      }
    }
  } else {
    if (z1 > z0) {
      if (x <= x0 && x >= x1 && z >= z0 && z <= z1) {
	return true;
      }
    } else {
      if (x <= x0 && x >= x1 && z <= z0 && z >= z1) {
	return true;
      }
    }
  }
  return false;
}

// threejs
function init() {

  container = document.createElement( 'div' );
  container.id = 'threejs-panel';
  document.body.appendChild( container );

  camera = new THREE.PerspectiveCamera( 45, container.offsetWidth / window.innerHeight, 1, 10000 );
  camera.position.set( 2000, 2500, 1000 );
  camera.lookAt( new THREE.Vector3() );

  // limit effect of mouse camera manipulation in container
  controls = new THREE.OrbitControls( camera, container, container );
  controls.target.x = board_width*pin_width/2;
  controls.target.z = board_height*pin_width/2;

  controls.addEventListener( 'change', render );

  scene = new THREE.Scene();

  // cubes
  cubeGeo = new THREE.BoxGeometry( pin_width, pin_width, pin_width );

  // grid

  //var size = pin_width0, step = pin_width;
  //var geometry = new THREE.Geometry();
  //for ( var i = 0; i <= board_height; i += 1 ) {
  //geometry.vertices.push( new THREE.Vector3(  0, 0, i*step ) );
  //geometry.vertices.push( new THREE.Vector3(   board_width*step, 0, i*step ) );
  //}
  //for ( var i = 0; i <= board_width; i += 1 ) {
  //geometry.vertices.push( new THREE.Vector3( i*step, 0, 0 ) );
  //geometry.vertices.push( new THREE.Vector3( i*step, 0,   board_height*step ) );
  //}
  //var material = new THREE.LineBasicMaterial( { color: 0x000000, opacity: 0.2, transparent: true } );
  //var line = new THREE.LineSegments( geometry, material );
  //scene.add( line );
  //line.name = 'thelines';

  raycaster = new THREE.Raycaster();
  onMouseDownPosition = new THREE.Vector2();
  onMouseUpPosition = new THREE.Vector2();
  onGhostMove = new THREE.Vector2();

  var geometry = new THREE.PlaneBufferGeometry( board_width*pin_width, board_height*pin_width );
  geometry.translate(board_width*pin_width/2, -board_height*pin_width/2, 0);
  geometry.rotateX( - Math.PI / 2 );

  plane = new THREE.Mesh( geometry, new THREE.MeshBasicMaterial( { visible: false } ) );
  scene.add( plane );
  plane.name = 'theplane';

  //objects.push( plane );

  // create default data to board_data
  create_new_board_data();

  // create pins according to board_data
  for (var i = 0; i < board_data.length; i+=1) {
    cubeMaterial = new THREE.MeshLambertMaterial( { color: pin_color} );
    var pin = new THREE.Mesh( cubeGeo, cubeMaterial );
    pin.translateX(board_data[i].col*pin_width + pin_width/2);
    pin.translateZ(board_data[i].row*pin_width + pin_width/2);
    pinSetHeight(pin, board_data[i].step_val);
    objects.push(pin);
    scene.add(pin);
  }

  // Lights

  var ambientLight = new THREE.AmbientLight( 0x606060 );
  scene.add( ambientLight );

  var directionalLight = new THREE.DirectionalLight( 0xffffff );
  directionalLight.position.set( 1, 0.75, 0.5 ).normalize();
  scene.add( directionalLight );

  renderer = new THREE.WebGLRenderer( { antialias: true } );
  renderer.setClearColor( 0xf0f0f0 );
  renderer.setPixelRatio( window.devicePixelRatio );
  renderer.setSize( window.innerWidth, window.innerHeight );
  container.appendChild( renderer.domElement );

  document.addEventListener( 'keydown', onDocumentKeyDown, false );
  document.addEventListener( 'keyup', onDocumentKeyUp, false );

  container.addEventListener( 'mousemove', onDocumentMouseMove, false );
  container.addEventListener( 'mousedown', onDocumentMouseDown, false );
  container.addEventListener( 'mouseup', onDocumentMouseUp, false );


  //
  window.addEventListener( 'resize', onWindowResize, false );
}

// not used
function makeGhost(color) {
  scene.remove( rollOverMesh );
  rollOverGeo = new THREE.BoxGeometry( pin_width, pin_width, pin_width );
  rollOverMaterial = new THREE.MeshBasicMaterial( { color: pin_color, opacity: 0.5, transparent: true } );
  rollOverMesh = new THREE.Mesh( rollOverGeo, rollOverMaterial );
  scene.add( rollOverMesh );
}

function onWindowResize() {
  camera.aspect = window.innerWidth / window.innerHeight;
  camera.updateProjectionMatrix();
  renderer.setSize( window.innerWidth, window.innerHeight );
}

function onDocumentMouseDown( event ) {
  event.preventDefault();
  onMouseDownPosition.set( ( event.clientX / window.innerWidth ) * 2 - 1, - ( event.clientY / window.innerHeight ) * 2 + 1 );

  if (event.button == 0) {
    isMouseLeftDown = true;
  }

  if (isMouseLeftDown) {

    raycaster.setFromCamera( onMouseDownPosition, camera );
    var intersects = raycaster.intersectObjects( objects );

    if ( intersects.length > 0 ) {
      var intersect = intersects[ 0 ];

      // selection
      if ( tool == 0 && selectedObjects != null && selectedObjects.includes(intersect.object) ) {
	if (tool == 0) {
	  // deselect
	  // if selected
	  if (selectedObjects.includes(intersect.object)) {
	    intersect.object.material.color.setHex( pin_color );
	    selectedObjects.splice( selectedObjects.indexOf( intersect.object ), 1 );
	    selectedObjectsHeight.splice( selectedObjects.indexOf( intersect.object ), 1 );
	  }
	}
      } else {
	if (tool == 0) {
	  // select
	  // if not selected
	  if (isShiftDown && selectedObjects != null) {
	    // area selection
	    var startPos = selectedObjects[selectedObjects.length-1].position;
	    for (var i = 0; i < objects.length; i += 1) {
	      if (isInArea(startPos, intersect.object.position, objects[i].position)) {
		if (!selectedObjects.includes(objects[i])) {
		  objects[i].material.color.setHex( selected_color );
		  selectedObjects.push(objects[i]);
		  selectedObjectsHeight.push(objects[i].scale.y*2);
		}
	      }
	    }
	  } else {
	    // per pin selection
	    if (!selectedObjects.includes(intersect.object)) {
	      intersect.object.material.color.setHex( selected_color );
	      selectedObjects.push(intersect.object);
	      selectedObjectsHeight.push(intersect.object.scale.y*2);

	      // debug information
	      var p_col = (intersect.object.position.x - pin_width/2)/pin_width;
	      var p_row = (intersect.object.position.z - pin_width/2)/pin_width;
	      var p_step_val = intersect.object.scale.y*2;
	      console.log("pin: col "+p_col.toString() + " row " + p_row.toString()+" step_val "+p_step_val.toString() + " | threejs position: ("+intersect.object.position.x.toString()+", "+intersect.object.position.y.toString()+", "+intersect.object.position.z.toString()+")");
	    }
	  }
	}
	else if (tool == 2 || tool == 3 || tool == 4) {
	  // just clear selected objects but keep the height changes
	  // remember the centered object to make adjustment
	  if (centeredObject == null) {
	    raycaster.setFromCamera( onGhostMove, camera );
	    var intersects = raycaster.intersectObjects( objects );
	    if ( intersects.length > 0 ) {
	      var intersect = intersects[ 0 ];
	      centeredObject = intersect.object;
	    }
	    if (selectedObjects != null) {
	      for (var i = 0; i < selectedObjects.length; i += 1) {
		selectedObjects[i].material.color.setHex( selected_color );
	      }
	    }
	  } else {
	    // something is already being selected, click on that to confirm, click on other place to dismiss
	    if (selectedObjects != null) {
	      raycaster.setFromCamera( onGhostMove, camera );
	      var intersects = raycaster.intersectObjects( objects );
	      if ( intersects.length > 0 ) {
		var intersect = intersects[ 0 ];
		if (selectedObjects.includes(intersect.object)) {
		  // confirm current changes
		  deselectAll();
		} else {
		  // dismiss current changes
		  resetSelectedObjects();
		}
		// no current selection now
		centeredObject.dispose;
		centeredObject = null;
	      }
	    }

	  }
	}
      }
    }

  }
  render();
}

function onDocumentMouseMove( event ) {

  event.preventDefault();

  onGhostMove.set( ( event.clientX / window.innerWidth ) * 2 - 1, - ( event.clientY / window.innerHeight ) * 2 + 1 );

  if (isMouseLeftDown) {
    // stroke select
    if (tool == 0) {

      raycaster.setFromCamera( onGhostMove, camera );
      var intersects = raycaster.intersectObjects( objects );

      if ( intersects.length > 0 ) {
	var intersect = intersects[ 0 ];

	if ( toolSelect == 0 ) {
	  // deselect
	  // if selected
	  if (selectedObjects.includes(intersect.object)) {
	    intersect.object.material.color.setHex( pin_color );
	    selectedObjects.splice( selectedObjects.indexOf( intersect.object ), 1 );
	    selectedObjectsHeight.splice( selectedObjects.indexOf( intersect.object ), 1 );
	  }
	} else {
	  // select if not selected
	  if (!selectedObjects.includes(intersect.object)) {
	    intersect.object.material.color.setHex( selected_color );
	    selectedObjects.push(intersect.object);
	    selectedObjectsHeight.push(intersect.object.scale.y*2);
	  }
	}
      }
      // extrude by mouse drag
      //if (selectedObjects != null) {
      //var posY = - ( event.clientY / window.innerHeight ) * 2 + 1;
      //var posDiff = posY - onMouseDownPosition.y;
      //var adjustedDiff = Math.round(posDiff*10);

      //for (var i = 0; i < selectedObjects.length; i += 1) {
      //pinAddHeight(selectedObjects[i], adjustedDiff);
      //}
      //}
    }
  }
  else {
    // mouse move when not clicking
    if (tool == 0) {
      // hover effect to show selected object
      raycaster.setFromCamera( onGhostMove, camera );
      var intersects = raycaster.intersectObjects( objects );

      resetHover();

      if ( intersects.length > 0 ) {
	var intersect = intersects[ 0 ];
	if (!(selectedObjects != null && selectedObjects.includes(intersect.object))) {
	  intersect.object.material.color.setHex( hover_color );
	  // area hovering
	  if (isShiftDown && selectedObjects != null && selectedObjects.length != 0) {
	    var startPos = selectedObjects[selectedObjects.length-1].position;
	    for (var i = 0; i < objects.length; i += 1) {
	      if (isInArea(startPos, intersect.object.position, objects[i].position)) {
		if (!selectedObjects.includes(objects[i])) {
		  objects[i].material.color.setHex( hover_color );
		}
	      }
	    }
	  }
	}
      }

    } else if (tool == 2 || tool == 3 || tool == 4) {
      // hole/hill tool
      if (centeredObject == null) {
	resetSelectedObjects();
	raycaster.setFromCamera( onGhostMove, camera );
	var intersects = raycaster.intersectObjects( objects );
	if ( intersects.length > 0 ) {
	  var intersect = intersects[ 0 ];
	  centeredObjectHeight = intersect.object.scale.y*2;
	  if (tool == 2) {
	    updateHolePreview(intersect.object, hover_color);
	  } else if (tool == 3) {
	    updateSlopePreview(intersect.object, hover_color);
	  } else if (tool == 4) {
	    updateTexturePreview(intersect.object, hover_color);
	  }
	}
      }
    }
  }

  render();

}

// not used
function onDocumentMouseUp (event) {

  event.preventDefault();

  isMouseLeftDown = false;

  onMouseUpPosition.set( ( event.clientX / window.innerWidth ) * 2 - 1, - ( event.clientY / window.innerHeight ) * 2 + 1 );

  if( onMouseDownPosition.distanceTo( onMouseUpPosition ) > 0.00001 ){ return; }


  raycaster.setFromCamera( onMouseUpPosition, camera );
  var intersects = raycaster.intersectObjects( objects );

  if ( intersects.length > 0 ) {
    var intersect = intersects[ 0 ];

    // delete cube
    if ( isShiftDown ) {
      //if ( intersect.object != plane ) {
      //scene.remove( intersect.object );
      //objects.splice( objects.indexOf( intersect.object ), 1 );
      //}

      // create cube
    } else {
      //cubeMaterial = new THREE.MeshLambertMaterial( { color: color} );
      //var voxel = new THREE.Mesh( cubeGeo, cubeMaterial );
      //voxel.position.copy( intersect.point ).add( intersect.face.normal );
      //voxel.position.divideScalar( pin_width ).floor().multiplyScalar( pin_width ).addScalar( 25 );

      //if (voxel.position.y > 0) {
      //scene.add( voxel );
      //objects.push( voxel );
      //}		

    }
    render();
  }
}

// only shift is used 
function onDocumentKeyDown( event ) {

  switch( event.keyCode ) {

    case 16: 
      isShiftDown = true; 
      break; 

  }

}

function onDocumentKeyUp( event ) {

  switch ( event.keyCode ) {

    case 16: isShiftDown = false; render(); break;

  }

}

// threejs function
function animate() {
  requestAnimationFrame( animate );
  controls.update();
}

// threejs function
function render() {
  renderer.render( scene, camera );
}

function pinSetHeight(pin, h) {
  if (h > 10) {
    h = 10;
  }
  if (h < 1) {
    h = 1;
  }
  h = h*0.5;
  pin.scale.set(1, h, 1);
  pin.position.setY(h/2*pin_width);
}

function pinAddHeight(pin, d) {
  var h = pin.scale.y*2;
  var new_h = h + d;
  if (new_h > 10) {
    new_h = 10;
  }
  if (new_h < 1) {
    new_h = 1;
  }
  pin.scale.set(1, new_h*0.5, 1);
  pin.position.setY(new_h*0.5*pin_width/2);
}

// not used
function downloadSTL() {

  scene.remove( rollOverMesh );
  scene.rotation.x = THREE.Math.degToRad(90);
  scene.scale.set(0.2,0.2,0.2);
  scene.updateMatrixWorld(); 

  saveSTL(scene, 'model');

  scene.scale.set(1,1,1);
  scene.rotation.x = 0;
  scene.add( rollOverMesh );
}

function downloadJSON() {

  board_data = []

  for (var i = 0; i < objects.length; i += 1) {
    var p_col = (objects[i].position.x - pin_width/2)/pin_width;
    var p_row = (objects[i].position.z - pin_width/2)/pin_width;
    var p_step_val = objects[i].scale.y*2;
    var p = {col: p_col, row: p_row, step_val: p_step_val}
    board_data.push(p);
  }

  var json = JSON.stringify({"board_width": board_width,"board_height": board_height, "board_data_list": board_data});

  //Convert JSON string to BLOB.
  json = [json];
  var blob1 = new Blob(json, { type: "text/plain;charset=utf-8" });

  //Check the Browser.
  var isIE = false || !!document.documentMode;
  if (isIE) {
    window.navigator.msSaveBlob(blob1, "saved_floor_data.txt");
  } else {
    var url = window.URL || window.webkitURL;
    link = url.createObjectURL(blob1);
    var a = document.createElement("a");
    a.download = "saved_floor_data.txt";
    a.href = link;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
  }
}

function loadFile() {
  var input, file, fr;

  if (typeof window.FileReader !== 'function') {
    alert("The file API isn't supported on this browser yet.");
    return;
  }

  input = document.getElementById('fileinput');
  if (!input) {
    alert("Um, couldn't find the fileinput element.");
  }
  else if (!input.files) {
    alert("This browser doesn't seem to support the `files` property of file inputs.");
  }
  else if (!input.files[0]) {
    alert("Please select a file before clicking 'Load'");
  }
  else {
    file = input.files[0];
    fr = new FileReader();
    fr.onload = receivedText;
    fr.readAsText(file);
  }

  function receivedText(e) {
    let lines = e.target.result;
    var newArr = JSON.parse(lines); 
    board_height = newArr.board_height;
    board_width = newArr.board_width;
    board_data = newArr.board_data_list;
    clear_board();
    create_board();
  }
}
