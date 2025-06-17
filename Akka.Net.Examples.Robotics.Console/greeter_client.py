import grpc
import grpc
import message_pb2
import message_pb2_grpc

def run():
    # The port number must match the port of the gRPC server.
    with grpc.insecure_channel("localhost:7042") as channel:
        client = greeter_pb2_grpc.GreeterStub(channel)
        try:
            response = client.SayHello(greeter_pb2.HelloRequest(name="GreeterClient"))
            print("Greeting: " + response.message)
        except grpc.RpcError as e:
            print(f"Error: {e.details}")

    print("Press any key to exit...")
    # In Python, we typically don't wait for a key press in console apps like C#
    # unless it's specifically for debugging or interaction in a script.
    # If you truly need to pause, you can use input().
    # input()

if __name__ == "__main__":
    run()