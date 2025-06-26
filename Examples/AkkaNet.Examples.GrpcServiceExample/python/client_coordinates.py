import grpc
import coordinates_pb2
import coordinates_pb2_grpc
import time
import random

def generate_points(num_points):
    """Generate a list of Point3D objects with random coordinates"""
    points = []
    for i in range(num_points):
        point = coordinates_pb2.Point3D(
            x=random.uniform(-100.0, 100.0),  # Random x between -100 and 100
            y=random.uniform(-100.0, 100.0),  # Random y between -100 and 100
            z=random.uniform(-100.0, 100.0)   # Random z between -100 and 100
        )
        points.append(point)
    return points

def create_batches(points, batch_size):
    """Split points into batches of specified size"""
    batches = []
    for i in range(0, len(points), batch_size):
        batch = points[i:i + batch_size]
        batches.append(batch)
    return batches

def run():
    print("Connecting to gRPC server at localhost:8080...")
    
    # Configuration
    total_points = 2000
    batch_size = 500
    
    try:
        # Generate all points
        print(f"Generating {total_points} 3D points...")
        all_points = generate_points(total_points)
        
        # Split into batches
        batches = create_batches(all_points, batch_size)
        print(f"Split into {len(batches)} batches of {batch_size} points each")
        
        # Connect to server
        with grpc.insecure_channel("localhost:8080") as channel:
            client = coordinates_pb2_grpc.CoordinateServiceStub(channel)
            
            # Track statistics
            total_sent = 0
            total_received = 0
            successful_batches = 0
            
            print(f"\nStarting batch processing...")
            overall_start_time = time.time()
            
            try:
                for batch_num, batch_points in enumerate(batches, 1):
                    print(f"\nProcessing batch {batch_num}/{len(batches)} ({len(batch_points)} points)...")
                    
                    # Create coordinate array for this batch
                    coordinate_array = coordinates_pb2.CoordinateArray(points=batch_points)
                    
                    # Send batch and measure time
                    batch_start_time = time.time()
                    response = client.SendCoordinates(coordinate_array)
                    batch_end_time = time.time()
                    
                    # Process response
                    batch_time = batch_end_time - batch_start_time
                    total_sent += len(batch_points)
                    total_received += response.points_received
                    successful_batches += 1
                    
                    print(f"Batch {batch_num} completed in {batch_time:.3f}s")
                    print(f"  Points sent: {len(batch_points)}")
                    print(f"  Points received by server: {response.points_received}")
                    print(f"  Server message: {response.message}")
                    
                    # Brief pause between batches (optional)
                    time.sleep(0.1)
                
                # Final statistics
                overall_end_time = time.time()
                total_time = overall_end_time - overall_start_time
                points_per_second = total_sent / total_time
                
                print(f"\n{'='*50}")
                print(f"BATCH PROCESSING COMPLETED")
                print(f"{'='*50}")
                print(f"Total points sent: {total_sent}")
                print(f"Total points received: {total_received}")
                print(f"Successful batches: {successful_batches}/{len(batches)}")
                print(f"Total processing time: {total_time:.2f} seconds")
                print(f"Rate: {points_per_second:.2f} points/second")
                print(f"Average batch time: {total_time/len(batches):.3f} seconds")
                
                if total_sent == total_received:
                    print("✅ All points successfully processed!")
                else:
                    print(f"⚠️  Point count mismatch: sent {total_sent}, received {total_received}")
                    
            except grpc.RpcError as e:
                status_code = e.code()
                print(f"RPC Error: {status_code.name} - {e.details()}")
                print(f"Successfully completed {successful_batches} batches before error")
            except Exception as e:
                print(f"Unexpected error: {str(e)}")
                print(f"Successfully completed {successful_batches} batches before error")
                
    except Exception as e:
        print(f"Failed to connect: {str(e)}")
 
    print("\nPress any key to exit...")
    input()
 
if __name__ == "__main__":
    run()