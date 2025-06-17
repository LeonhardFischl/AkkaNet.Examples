import grpc
import greet_pb2
import greet_pb2_grpc

def run():
    print("Connecting to gRPC server at localhost:5077...")
    
    try:
        # The port number must match the port of the gRPC server.
        with grpc.insecure_channel("localhost:5077") as channel:
            client = greet_pb2_grpc.GreeterStub(channel)
            try:
                print("Sending request to SayHello...")
                response = client.SayHello(greet_pb2.HelloRequest(name="GreeterClient"))
                print("Greeting: " + response.message)
            except grpc.RpcError as e:
                status_code = e.code()
                print(f"RPC Error: {status_code.name} - {e.details()}")
            except Exception as e:
                print(f"Unexpected error: {str(e)}")
    except Exception as e:
        print(f"Failed to connect: {str(e)}")

    print("Press any key to exit...")
    # Uncomment to pause execution until user presses Enter
    input()

if __name__ == "__main__":
    run()