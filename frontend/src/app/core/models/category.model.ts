export interface Category {
  id: string;
  name: string;
  description: string | null;
  createdAt: string;
}

export interface CategoryRequest {
  name: string;
  description: string | null;
}
