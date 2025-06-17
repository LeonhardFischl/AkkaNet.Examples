import grpc
import message_pb2
import message_pb2_grpc
import server_pb2
import server_pb2_grpc

def run():
    # Connect to the TCP endpoint (replace with your server address)
    channel = grpc.insecure_channel('localhost:8080')
    #stub = message_pb2_grpc.MessageServiceStub(channel)
    stub = server_pb2_grpc.ChatServerStub(channel)

    # Prepare your message
    #request = message_pb2.MessageRequest(content="Hello, TCP endpoint via gRPC!")
    
    start_request = server_pb2.ClientMessageStart(message="Initialize gRPC!")

    customer_request = server_pb2.ClientMessageCustomer(message="Hello, TCP endpoint via gRPC!")
    
    stop_request = server_pb2.ClientMessageStop(message="stop the server gRPC!")
    
    # Send the message and receive the reply
    #response = stub.SendMessage(request)
    #print("Server replied:", response.status)
    summary = stub.HandleCommunication(start_request)

    summary = stub.HandleCommunication(customer_request)

    summary = stub.HandleCommunication(stop_request)

if __name__ == "__main__":
    run()


# class MyClient:
#     def __init__(self, address, port):
#         self.address = address
#         self.port = port
#         #create a grpc channel

#     def connect(self):
#         #connect the channel

#     def read(self):
#         #read the channel