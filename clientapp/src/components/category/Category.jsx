import React, { useState, useEffect } from "react";
import { useDispatch } from "react-redux";
import { useQuery } from "@apollo/client";
import { GET_CATEGORIES, GET_POSTS } from "../../features/graphql/Queries";
import { logout } from "../../features/auth/authSlice";
import "./Category.css";

const Category = () => {

  const [selectedCategory, setSelectedCategory] = useState(null);
  const [page, setPage] = useState(1);
  const [maxPages, setMaxPages] = useState(1);
  const dispatch = useDispatch();

  // Fetch categories for the select dropdown
  const { loading: categoriesLoading, error: categoriesError, data: categoriesData } = useQuery(GET_CATEGORIES);

  // Fetch posts based on selected category and pagination
  const { loading: postsLoading, error: postsError, data: postsData, refetch } = useQuery(GET_POSTS, {
    variables: { categoryId: selectedCategory?.id || null, page },
    skip: !selectedCategory,  // Skip query until a category is selected
  });

  // Handle category selection
  const handleCategoryChange = (event) => {
    const category = categoriesData.categories.find((c) => c.id === event.target.value);
    setSelectedCategory(category);
    console.log(Math.ceil(category.count/10))
    setMaxPages(Math.ceil(category.count/10));
    setPage(1); 
  };

  // Handle pagination (go to next/previous page)
  const handleNextPage = () => {
    if (page < maxPages) {
      setPage(page + 1);
    }
  };

  const handlePreviousPage = () => {
    if (page > 1) {
      setPage(page - 1);
    }
  };

  const renderPosts = () => {
    if(!postsData)return <p>No posts</p>
    return <div>
    <ul class="posts">
      {postsData.posts.map((post) => (
        <li key={post.slug}>
          <a href={post.link} target="_blank">{post.title}</a>
          <small>{post.link}</small>
        </li>
      ))}
    </ul>
    {/* Pagination Controls */}
    <div className="pagination">
      <button onClick={handlePreviousPage} disabled={page === 1}>
        Previous
      </button>
      <span>
        Page {page} of {maxPages}
      </span>
      <button onClick={handleNextPage} disabled={page >= maxPages}>
        Next
      </button>
    </div>
  </div>
  }

  // Re-fetch posts when page changes
  useEffect(() => {
    if (selectedCategory) {
      refetch();
    }
  }, [page, selectedCategory, refetch]);

  if (categoriesLoading) return <p>Loading categories...</p>;
  if (categoriesError) return <p>Error loading categories</p>;

  const handleLogout = () => {
    dispatch(logout());  
  };

  return (
    <div className="container">
      <h1>Browse Posts by Category</h1>
      <h5>or <a href="#" onClick={handleLogout}>Logout</a></h5>

      {/* Category selection dropdown */}
      <select onChange={handleCategoryChange} value={selectedCategory || ""}>
        <option value="" disabled>
          Select a category
        </option>
        {categoriesData.categories.map((category) => (
          <option key={category.id} value={category.id}>
            {category.name}
          </option>
        ))}
      </select>

      {/* Display posts */}
      {postsLoading ? (
        <p>Loading posts...</p>
      ) : postsError ? (
        <p>Error loading posts</p>
      ) : (
        renderPosts()
      )}
    </div>
  );
};

export default Category;
