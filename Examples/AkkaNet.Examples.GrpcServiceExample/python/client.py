import grpc
import greet_pb2
import greet_pb2_grpc
import time
 
def run():
    print("Connecting to gRPC server at localhost:8080)...")
    
    try:
        # The port number must match the port of the gRPC server.
        with grpc.insecure_channel("localhost:8080") as channel:
            client = greet_pb2_grpc.GreeterStub(channel)
            try:
                print("Sending 20000 requests...")
                start_time = time.time()
                
                for i in range(60000):
                    response = client.SayHello(greet_pb2.HelloRequest(name=f"GreeterClient-{i}"))
                    if i % 1000 == 0:  # Print progress every 1000 messages
                        print(f"Sent {i} messages, received: {response.message}")
                
                end_time = time.time()
                elapsed_time = end_time - start_time
                messages_per_second = 20000 / elapsed_time
                print(f"Completed 20000 messages in {elapsed_time:.2f} seconds")
                print(f"Rate: {messages_per_second:.2f} messages/second")
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