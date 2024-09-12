import { gql } from "@apollo/client";

// Query to fetch categories
export const GET_CATEGORIES = gql`
  query GetCategories {
    categories {
      id
      name
      count
    }
  }
`;

// Query to fetch posts with pagination and category filter
export const GET_POSTS = gql`
  query GetPosts($categoryId: String!, $page: Int!) {
    posts(categoryId: $categoryId, page: $page) {  
        title
        slug
        link
    }
  }
`;
