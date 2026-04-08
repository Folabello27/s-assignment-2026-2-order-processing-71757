import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export interface Order {
  orderId: number;
  customerId: string;
  name: string;
  orderStatus: string;
  totalAmount: number;
  itemCount: number;
  createdAt: string;
  updatedAt?: string;
}

export interface PaginatedOrdersResponse {
  orders: Order[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface DashboardSummary {
  totalOrders: number;
  pendingOrders: number;
  completedOrders: number;
  failedOrders: number;
  totalRevenue: number;
  ordersByStatus: { [key: string]: number };
}

export interface OrderStatus {
  orderId: number;
  status: string;
  createdAt: string;
  updatedAt?: string;
  inventoryStatus?: string;
  paymentStatus?: string;
  shippingStatus?: string;
}

export const orderApi = {
  getOrders: async (page = 1, pageSize = 20, status?: string): Promise<PaginatedOrdersResponse> => {
    const params = new URLSearchParams();
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());
    if (status) params.append('status', status);

    const response = await api.get(`/orders?${params.toString()}`);
    return response.data;
  },

  getOrderById: async (id: number): Promise<Order> => {
    const response = await api.get(`/orders/${id}`);
    return response.data;
  },

  getOrderStatus: async (id: number): Promise<OrderStatus> => {
    const response = await api.get(`/orders/${id}/status`);
    return response.data;
  },

  getDashboardSummary: async (): Promise<DashboardSummary> => {
    const response = await api.get('/orders/dashboard/summary');
    return response.data;
  },

  getOrdersByStatus: async (status: string, page = 1, pageSize = 20): Promise<PaginatedOrdersResponse> => {
    const response = await api.get(`/orders/by-status/${status}?page=${page}&pageSize=${pageSize}`);
    return response.data;
  },
};
