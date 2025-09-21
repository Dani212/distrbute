export interface APIResponse<T> {
  message: string;
  code: number;
  data: T;
}

export interface APIListResponse<T> {
  message: string;
  code: number;
  data: {
    page: number;
    pageSize: number;
    totalCount: number;
    data: T[];
  };
  errors?: string | null;
}
