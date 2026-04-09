import React, { useEffect, useState } from 'react';
import { Table, Spinner, Button } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { Order, orderApi } from '../api/orderApi';

const FailedOrders: React.FC = () => {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchFailedOrders();
  }, []);

  const fetchFailedOrders = async () => {
    try {
      const data = await orderApi.getOrdersByStatus('Failed');
      setOrders(data.orders);
    } catch {
      setError('Unable to connect to Order API');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="text-center mt-5">
        <Spinner animation="border" role="status">
          <span className="visually-hidden">Loading...</span>
        </Spinner>
      </div>
    );
  }

  if (error) {
    return (
      <div className="alert alert-warning" role="alert">
        {error}
      </div>
    );
  }

  return (
    <div>
      <h2 className="mb-4">Failed Orders</h2>
      {orders.length === 0 ? (
        <div className="alert alert-success">
          No failed orders found. Great news!
        </div>
      ) : (
        <Table striped bordered hover responsive>
          <thead>
            <tr>
              <th>Order ID</th>
              <th>Customer</th>
              <th>Customer ID</th>
              <th>Total</th>
              <th>Status</th>
              <th>Date</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {orders.map(order => (
              <tr key={order.orderId}>
                <td>{order.orderId}</td>
                <td>{order.name}</td>
                <td>{order.customerId}</td>
                <td>${order.totalAmount.toFixed(2)}</td>
                <td>{order.orderStatus}</td>
                <td>{new Date(order.createdAt).toLocaleDateString()}</td>
                <td>
                  <Link to={`/orders/${order.orderId}`}>
                    <Button variant="primary" size="sm">View Details</Button>
                  </Link>
                </td>
              </tr>
            ))}
          </tbody>
        </Table>
      )}
    </div>
  );
};

export default FailedOrders;
