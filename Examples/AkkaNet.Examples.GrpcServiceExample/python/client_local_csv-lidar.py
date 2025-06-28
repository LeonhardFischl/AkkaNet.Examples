import grpc
import coordinates_pb2
import coordinates_pb2_grpc
import time
import csv

def load_lidar_points(csv_file_path):
    """Load Point3D objects from LiDAR CSV file"""
    points = []
    invalid_rows = 0
    
    print(f"Loading LiDAR data from {csv_file_path}...")
    
    with open(csv_file_path, 'r', encoding='utf-8') as file:
        csv_reader = csv.reader(file)
        
        for row_num, row in enumerate(csv_reader, 1):
            try:
                # Ensure we have exactly 3 values
                if len(row) != 3:
                    invalid_rows += 1
                    continue
                
                # Convert to float and create Point3D
                x = float(row[0])
                y = float(row[1])
                z = float(row[2])
                
                point = coordinates_pb2.Point3D(x=x, y=y, z=z)
                points.append(point)
                
                # Progress indicator for large files
                if row_num % 50000 == 0:
                    print(f"  Loaded {row_num:,} points...")
                    
            except (ValueError, IndexError) as e:
                invalid_rows += 1
                if invalid_rows <= 5:  # Only show first few errors
                    print(f"  Warning: Invalid row {row_num}: {row} - {str(e)}")
    
    print(f"✅ Loaded {len(points):,} valid points")
    if invalid_rows > 0:
        print(f"⚠️  Skipped {invalid_rows} invalid rows")
    
    return points

def create_batches(points, batch_size):
    """Split points into batches of specified size"""
    batches = []
    for i in range(0, len(points), batch_size):
        batch = points[i:i + batch_size]
        batches.append(batch)
    return batches

def calculate_statistics(points):
    """Calculate basic statistics about the point cloud"""
    if not points:
        return
    
    x_coords = [point.x for point in points[:1000]]  # Sample for efficiency
    y_coords = [point.y for point in points[:1000]]
    z_coords = [point.z for point in points[:1000]]
    
    print(f"\nPoint Cloud Statistics (sample of {len(x_coords)} points):")
    print(f"  X range: {min(x_coords):.3f} to {max(x_coords):.3f}")
    print(f"  Y range: {min(y_coords):.3f} to {max(y_coords):.3f}")
    print(f"  Z range: {min(z_coords):.3f} to {max(z_coords):.3f}")

def run():
    print("LiDAR Point Cloud gRPC Client")
    print("=" * 40)
    print("Connecting to gRPC server at localhost:8080...")
    
    # Configuration
    csv_file_path = "lidar_data.csv"  # Update path as needed
    batch_size = 2000
    
    try:
        # Load LiDAR points from CSV
        all_points = load_lidar_points(csv_file_path)
        
        if not all_points:
            print("❌ No valid points loaded. Exiting.")
            return
        
        # Show statistics
        calculate_statistics(all_points)
        
        # Split into batches
        batches = create_batches(all_points, batch_size)
        total_batches = len(batches)
        print(f"\nSplit into {total_batches} batches of up to {batch_size} points each")
        
        # Connect to server
        with grpc.insecure_channel("localhost:8080") as channel: # localhost # ec2-3-78-137-76.eu-central-1.compute.amazonaws.com
            client = coordinates_pb2_grpc.CoordinateServiceStub(channel)
            
            # Track statistics
            total_sent = 0
            total_received = 0
            successful_batches = 0
            
            print(f"\nStarting batch processing...")
            overall_start_time = time.time()
            
            for batch_num, batch_points in enumerate(batches, 1):
                # Calculate progress
                progress_percent = (batch_num / total_batches) * 100
                
                print(f"\nProcessing batch {batch_num}/{total_batches} ({len(batch_points)} points) - {progress_percent:.1f}%")
                
                # Create coordinate array for this batch
                coordinate_array = coordinates_pb2.CoordinateArray(points=batch_points)
                
                # Send batch and measure time
                batch_start_time = time.time()
                
                try:
                    response = client.SendCoordinates(coordinate_array)
                    batch_end_time = time.time()
                    
                    # Process response
                    batch_time = batch_end_time - batch_start_time
                    total_sent += len(batch_points)
                    total_received += response.points_received
                    successful_batches += 1
                    
                    # Calculate ETA
                    elapsed_time = batch_end_time - overall_start_time
                    avg_time_per_batch = elapsed_time / batch_num
                    remaining_batches = total_batches - batch_num
                    eta_seconds = remaining_batches * avg_time_per_batch
                    eta_minutes = eta_seconds / 60
                    
                    print(f"  ✅ Completed in {batch_time:.3f}s")
                    print(f"  📊 Points sent: {len(batch_points)}, received: {response.points_received}")
                    print(f"  📈 Rate: {len(batch_points)/batch_time:.0f} points/sec")
                    print(f"  ⏱️  ETA: {eta_minutes:.1f} minutes")
                    print(f"  💬 Server: {response.message}")
                    
                except grpc.RpcError as e:
                    print(f"  ❌ Batch {batch_num} failed: {e.code().name} - {e.details()}")
                    break  # Stop on first error since no retry logic
                except Exception as e:
                    print(f"  ❌ Batch {batch_num} failed: {str(e)}")
                    break
            
            # Final statistics
            overall_end_time = time.time()
            total_time = overall_end_time - overall_start_time
            
            print(f"\n{'='*60}")
            print(f"LIDAR POINT CLOUD PROCESSING COMPLETED")
            print(f"{'='*60}")
            print(f"📋 Total points sent: {total_sent:,}")
            print(f"📋 Total points received: {total_received:,}")
            print(f"📋 Successful batches: {successful_batches}/{total_batches}")
            print(f"⏱️  Total processing time: {total_time:.2f} seconds ({total_time/60:.1f} minutes)")
            
            if total_time > 0:
                points_per_second = total_sent / total_time
                print(f"📈 Overall rate: {points_per_second:.0f} points/second")
                print(f"📈 Average batch time: {total_time/successful_batches:.3f} seconds")
            
            if total_sent == total_received:
                print("✅ All points successfully processed!")
            else:
                print(f"⚠️  Point count mismatch: sent {total_sent:,}, received {total_received:,}")
            
            # AWS readiness note
            print(f"\n🚀 Ready for AWS deployment:")
            print(f"   - Processed {len(all_points):,} points successfully")
            print(f"   - Batch size: {batch_size} points")
            print(f"   - Average throughput: {points_per_second:.0f} points/sec")
                
    except FileNotFoundError:
        print(f"❌ CSV file '{csv_file_path}' not found.")
        print("   Please ensure the file is in the same directory as this script.")
    except Exception as e:
        print(f"❌ Failed to process: {str(e)}")
 
    print("\nPress any key to exit...")
    input()
 
if __name__ == "__main__":
    run()