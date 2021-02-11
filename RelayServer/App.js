const net = require('net');

const robot_port = 21;
const unity_port = 27;

const robot_client_interface = net.createServer().listen(robot_port, () => {
    console.log("Start listening from robot on port " + robot_port);
});


const unity_client_interface = net.createServer().listen(unity_port, () => {
    console.log("Start listening from unity on port " + unity_port);
});

var robot_client = null;
var unity_client = null;

robot_client_interface.on('connection', (robot_client_socket) => {
    robot_client= robot_client_socket;
    console.log("Connection: from robot: " + robot_client.remoteAddress + ' : ' + robot_client.remotePort);

    robot_client_socket.on('data', (data) => {
        console.log("receive joint state from robot: " + data);
        if (unity_client != null) {
            unity_client.write(data + '\n');
            console.log("relaying joint state to unity: " + data);
        }
    });

    robot_client_socket.on('error', (err) => {
        console.log('error from robot: ' + err);
    })

    robot_client_socket.on('close', (data) => {
        robot_client = null;
        console.log("robot client disconnected: " + data);
    });
});

unity_client_interface.on('connection', (unity_client_socket) => {
    unity_client = unity_client_socket;
    console.log("Connection: from unity:  " + unity_client.remoteAddress + ' : ' + unity_client.remotePort);

    unity_client_socket.on('data', (data) => {
        console.log("receive pos from unity: " + data);
        if (robot_client != null) {
            robot_client.write(data);
            console.log("relaying pos to robot: " + data);
        }
    });

    unity_client_socket.on('close', (data) => {
        unity_client = null;
        console.log("unity client disconnected: " + data);
    });
});
