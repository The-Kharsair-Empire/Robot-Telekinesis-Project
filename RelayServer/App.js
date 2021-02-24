const net = require('net');

const robot_port_primary = 21;
const robot_port_secondary = 22;
const unity_port_primary = 27;
const unity_port_secondary = 28;

const robot_client_handle_primary = net.createServer().listen(robot_port_primary, () => {
    console.log("Start listening from robot on port " + robot_port_primary);
});


const unity_client_handle_primary = net.createServer().listen(unity_port_primary, () => {
    console.log("Start listening from unity on port " + unity_port_primary);
});

const robot_client_handle_secondary = net.createServer().listen(robot_port_secondary, () => {
    console.log("Start listening from robot on port " + robot_port_secondary);
});

const unity_client_handle_secondary = net.createServer().listen(unity_port_secondary, () => {
    console.log("Start listening from unity on port " + unity_port_secondary);
});


var robot_client_primary = null;
var robot_client_secondary = null;
var unity_client_primary = null;
var unity_client_secondary = null;


robot_client_handle_secondary.on('connection', (robot_client_socket) => {
    robot_client_secondary= robot_client_socket;
    console.log("Connection: from robot primary: " + robot_client_secondary.remoteAddress + ' : ' + robot_client_secondary.remotePort);

    robot_client_socket.on('data', (data) => {
        console.log("receive joint state from robot: " + data);
        if (unity_client_secondary != null) {
            unity_client_secondary.write(data + '\n');
            console.log("relaying joint state to unity secondary: " + data);
        }
    });

    robot_client_socket.on('error', (err) => {
        console.log('error from robot primary: ' + err);
    })

    robot_client_socket.on('close', (data) => {
        robot_client_secondary = null;
        console.log("robot client primary disconnected: " + data);
    });
});

robot_client_handle_primary.on('connection', (robot_client_socket) => {
    robot_client_primary = robot_client_socket;
    console.log("Connection: from robot secondary: " + robot_client_primary.remoteAddress + ' : ' + robot_client_primary.remotePort);

    robot_client_socket.on('data', (data) => {
        
        if (data[0] == "p") {
            console.log("receive actual pos from robot: " + data);
            if (unity_client_primary != null) {
                unity_client_primary.write(data + '\n');
                console.log("relaying pos to unity primary: " + data);
            }
        }else {
            console.log("receive inverse kinematic plan from robot: " + data);
            //handle inverse kinematics by sending back servoj to robot
        }
        

    })

    robot_client_socket.on('error', (err) => {
        console.log('error from robot secondary: ' + err);
    })

    robot_client_socket.on('close', (data) => {
        robot_client_primary = null;
        console.log("robot client secondary disconnected: " + data);
    });
});

unity_client_handle_primary.on('connection', (unity_client_socket) => {
    unity_client_primary = unity_client_socket;
    console.log("Connection: from unity primary:  " + unity_client_primary.remoteAddress + ' : ' + unity_client_primary.remotePort);

    unity_client_socket.on('data', (data) => {
        console.log("receive pos from unity: " + data);
        if (robot_client_primary != null) {
            robot_client_primary.write(data);
            
            console.log("relaying pos to robot: " + data);
        }
    });

    unity_client_socket.on('close', (data) => {
        unity_client_primary = null;
        console.log("unity primary client disconnected: " + data);
    });
});

unity_client_handle_secondary.on('connection', (unity_client_socket) => {
    unity_client_secondary = unity_client_socket;
    console.log('Connection: from unity secondary: '  + unity_client_secondary.remoteAddress + ' : ' + unity_client_secondary.remotePort);

    unity_client_socket.on('data', (data) => {
        console.log("YOU ARE NOT SUPPOSED TO RECEIVE DATA FROM THIS PORT: " + data)
    })
    unity_client_socket.on('close', (data) => {
        unity_client_secondary = null;
        console.log("unity secondary client disconnected: " + data);
    })
})
