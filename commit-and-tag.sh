#!/bin/bash

# Function to get the latest tag
get_latest_tag() {
    # Get the latest tag, fallback to 0.0.0 if no tags exist
    local latest_tag=$(git describe --tags --abbrev=0 2>/dev/null || echo "0.0.0")
    echo "$latest_tag"
}

# Function to increment version
increment_version() {
    local version=$1
    local type=$2
    
    # Remove 'v' prefix if it exists
    version=$(echo "$version" | sed 's/^v//')
    
    # Split version into parts
    IFS='.' read -ra VERSION_PARTS <<< "$version"
    local major=${VERSION_PARTS[0]:-0}
    local minor=${VERSION_PARTS[1]:-0}
    local patch=${VERSION_PARTS[2]:-0}
    
    case $type in
        "major")
            major=$((major + 1))
            minor=0
            patch=0
            ;;
        "minor")
            minor=$((minor + 1))
            patch=0
            ;;
        "patch")
            patch=$((patch + 1))
            ;;
        *)
            echo "Invalid version type. Use: major, minor, or patch"
            exit 1
            ;;
    esac
    
    echo "$major.$minor.$patch"
}

# Function to detect version type based on commit messages
detect_version_type() {
    local current_tag=$1
    
    # Get commits since last tag
    local commits
    if [ "$current_tag" = "0.0.0" ]; then
        # If no previous tags, get all commits
        commits=$(git log --oneline --pretty=format:"%s")
    else
        commits=$(git log --oneline --pretty=format:"%s" "$current_tag"..HEAD)
    fi
    
    # Check for breaking changes (major version)
    if echo "$commits" | grep -qiE "(BREAKING CHANGE|!:|breaking:|major:)"; then
        echo "major"
        return
    fi
    
    # Check for new features (minor version)
    if echo "$commits" | grep -qiE "(feat:|feature:|minor:|add:|new:)"; then
        echo "minor"
        return
    fi
    
    # Default to patch for bug fixes and other changes
    echo "patch"
}

# Main script
main() {
    # Check if there are any changes to commit
    if git diff --cached --quiet; then
        echo "No staged changes to commit. Please stage your changes first with 'git add'."
        exit 1
    fi
    
    # Get commit message
    if [ $# -eq 0 ]; then
        echo "Please provide a commit message:"
        read -r commit_message
    else
        commit_message="$*"
    fi
    
    # Get current latest tag
    current_tag=$(get_latest_tag)
    echo "Current latest tag: $current_tag"
    
    # Ask user for version type or auto-detect
    echo ""
    echo "Version increment options:"
    echo "1. patch   (bug fixes, small changes)     - $current_tag -> $(increment_version "$current_tag" "patch")"
    echo "2. minor   (new features, backwards compatible) - $current_tag -> $(increment_version "$current_tag" "minor")"
    echo "3. major   (breaking changes)             - $current_tag -> $(increment_version "$current_tag" "major")"
    echo "4. auto    (detect from commit messages)"
    echo ""
    echo "Choose version type (1-4) or press Enter for auto-detection:"
    read -r version_choice
    
    case $version_choice in
        "1")
            version_type="patch"
            ;;
        "2")
            version_type="minor"
            ;;
        "3")
            version_type="major"
            ;;
        "4"|"")
            version_type=$(detect_version_type "$current_tag")
            echo "Auto-detected version type: $version_type"
            ;;
        *)
            echo "Invalid choice. Using auto-detection."
            version_type=$(detect_version_type "$current_tag")
            echo "Auto-detected version type: $version_type"
            ;;
    esac
    
    # Calculate new version
    new_version=$(increment_version "$current_tag" "$version_type")
    echo "New version will be: $new_version"
    
    # Confirm before tagging
    echo ""
    echo "Ready to create tag '$new_version'. Continue? (y/N)"
    read -r confirm
    
    if [[ $confirm =~ ^[Yy]$ ]]; then
        # Commit the changes
        echo "Committing changes..."
        git commit -m "$commit_message" || { echo "Failed to commit changes"; exit 1; }
            
        # Create the tag
        git tag "$new_version" || { echo "Failed to create tag"; exit 1; }
        echo "✅ Created tag: $new_version"
        
        # Ask if user wants to push
        echo ""
        echo "Push commits and tags to remote? (y/N)"
        read -r push_confirm
        
        if [[ $push_confirm =~ ^[Yy]$ ]]; then
            git push && git push --tags
            echo "✅ Pushed commits and tags to remote"
        else
            echo "ℹ️  Don't forget to push when ready: git push && git push --tags"
        fi
    else
        echo "❌ Tagging cancelled"
        exit 1
    fi
}

# Run main function
main "$@"
