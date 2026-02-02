# GitHub Actions Workflow Fix Documentation

## Problem Summary
The "Build and Release" workflow was not triggering when releases were created or published, despite having the correct trigger configuration.

## Root Causes Identified

### 1. Release Event Type Configuration
- **Previous**: Used `types: [created, published]`
- **Issue**: The `created` event type can be less reliable
- **Fix**: Changed to `types: [published]` which is the recommended approach

According to GitHub documentation, `published` is the most reliable event type for release workflows as it triggers when:
- A release is published (not a draft)
- A pre-release is marked as ready for production

### 2. Lack of Debugging Information
- **Previous**: No event logging
- **Issue**: Impossible to diagnose why workflow wasn't triggering
- **Fix**: Added comprehensive debug step that logs:
  - Event name and action
  - Repository and commit information
  - Release-specific details when applicable

### 3. Hard-coded Release Context
- **Previous**: Used `${{ github.event.release.tag_name }}` directly
- **Issue**: Would fail on non-release triggers (testing, manual runs)
- **Fix**: Dynamic version handling that adapts to event type

## Changes Made

### 1. Updated Trigger Configuration
```yaml
on:
  release:
    types: [published]  # Changed from [created, published]
  workflow_dispatch:
  push:
    branches:
      - main
    paths:
      - '.github/workflows/build-and-release.yml'
```

**Why**: 
- `published` is more reliable and recommended by GitHub
- Added `push` trigger limited to workflow file changes for testing
- This allows verification that the workflow runs at all

### 2. Added Debug Event Information Step
```yaml
- name: Debug Event Information
  run: |
    echo "Event Name: ${{ github.event_name }}"
    echo "Event Action: ${{ github.event.action }}"
    # ... more debug output
```

**Why**: Provides visibility into what triggered the workflow and helps diagnose issues

### 3. Dynamic Version Handling
```yaml
- name: Create zip archives
  run: |
    if ("${{ github.event_name }}" -eq "release") {
      $version = "${{ github.event.release.tag_name }}"
    } else {
      $version = "${{ github.sha }}".Substring(0, 7)
    }
```

**Why**: Allows workflow to run successfully for testing without requiring a release

### 4. Conditional Upload Steps
```yaml
- name: Upload release assets
  if: github.event_name == 'release'
  # ...

- name: Upload build artifacts (for testing)
  if: github.event_name != 'release'
  # ...
```

**Why**: Prevents errors when release context isn't available

## How to Verify the Fix

### Test 1: Manual Trigger (Immediate)
1. Go to Actions tab in GitHub
2. Select "Build and Release" workflow
3. Click "Run workflow"
4. Check the workflow runs and review the "Debug Event Information" output

### Test 2: Push Trigger (After Merge)
1. Merge this PR to main
2. The workflow should automatically trigger
3. Check workflow runs in Actions tab
4. Verify debug output shows `event_name: push`

### Test 3: Release Trigger (Final Validation)
1. Create a new release in GitHub:
   - Go to Releases → Draft a new release
   - Create a new tag (e.g., `v1.0.0`)
   - Fill in release title and description
   - Click "Publish release"
2. Workflow should trigger automatically
3. Check workflow runs in Actions tab
4. Verify debug output shows `event_name: release`
5. Confirm zip files are attached to the release

## Common Issues and Solutions

### Issue: Workflow still not triggering on release
**Possible Causes**:
1. **Workflow not on main branch**: Ensure this change is merged to main
2. **Release created by GitHub Action**: If another workflow creates the release using `GITHUB_TOKEN`, it won't trigger this workflow (security feature)
   - **Solution**: Use a Personal Access Token (PAT) instead of `GITHUB_TOKEN` in the creating workflow
3. **Repository Settings**: Check Actions permissions in Settings → Actions → General

### Issue: Workflow runs but fails
**Debug Steps**:
1. Check the "Debug Event Information" step output
2. Verify the event type and available context
3. Check if all required files exist in the repository
4. Ensure .NET SDK 8.0 is available (should be by default on windows-latest)

## Best Practices Applied

1. **Use `published` instead of `created`**: More reliable for production releases
2. **Add debug logging**: Always include event context logging for troubleshooting
3. **Conditional steps**: Use `if` conditions to handle different trigger types
4. **Testing triggers**: Include push/workflow_dispatch for testing without creating releases
5. **Dynamic values**: Calculate values based on context instead of hard-coding

## Additional Notes

- The workflow file must be present on the **default branch (main)** for release events to trigger it
- GitHub Actions has a security feature that prevents workflows from triggering other workflows when using `GITHUB_TOKEN`
- The `published` event type is preferred over `created` because it only triggers for finalized releases, not drafts

## References

- [GitHub Actions: Events that trigger workflows](https://docs.github.com/en/actions/reference/events-that-trigger-workflows#release)
- [GitHub Actions: Workflow syntax](https://docs.github.com/en/actions/reference/workflow-syntax-for-github-actions)
- [Troubleshooting workflow triggers](https://docs.github.com/en/actions/managing-workflow-runs/re-running-workflows-and-jobs)
