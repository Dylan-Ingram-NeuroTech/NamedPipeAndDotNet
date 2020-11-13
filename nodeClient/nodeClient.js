var net = require('net');

var PIPE_NAME = "TestPipe";
var PIPE_PATH = "\\\\.\\pipe\\" + PIPE_NAME;
var L = console.log;

// == Client == //
var client = net.connect(PIPE_PATH, function () {
    L('Client: on connection');
})

client.on('data', function (data) {
    var data = new Uint8Array(data);
    var f32 = new Float32Array(data.buffer);

    L('Client: on data:', "S1: " + f32[0] + " S2: " + f32[1] + " S3: " + f32[2] + " S4: " + f32[3]
        + " S5: " + f32[4] + " S6: " + f32[5] + " S7: " + f32[6] + " S8: " + f32[7] + " time: " + f32[8])
});

client.on('end', function () {
    L('Client: on end');
})