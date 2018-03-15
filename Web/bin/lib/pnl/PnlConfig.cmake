# we assume that PnlConfig is located in <prefix>/lib/pnl/
get_filename_component(pnl_root_dir "${CMAKE_CURRENT_LIST_DIR}/../.." ABSOLUTE)


if (MSVC)
    set(PNL_LIBRARY ${pnl_root_dir}/lib/libpnl.lib)
else (MSVC)
    set(PNL_LIBRARY ${pnl_root_dir}/lib/libpnl.dll)
endif(MSVC)
set(PNL_INCLUDE_DIR ${pnl_root_dir}/include)

# Handle the QUIETLY and REQUIRED arguments and set PNL_FOUND to TRUE if
# all listed variables are TRUE
include(FindPackageHandleStandardArgs)
find_package_handle_standard_args(Pnl
    REQUIRED_VARS 
        PNL_LIBRARY PNL_INCLUDE_DIR
    FAIL_MESSAGE 
    "Pnl not found. Consider defining Pnl_DIR."
)

function(pnl_add_postbuild target)
    # Copy all the .dll to the binary build dir
	if(MSVC)
		set(components libpnl.dll libblas.dll liblapack.dll libgcc_s_seh-1.dll libquadmath-0.dll libgfortran-3.dll)
		get_filename_component(PNL_LIB_DIRECTORY ${PNL_LIBRARY} DIRECTORY)
        foreach(entry ${components})
            add_custom_command(TARGET ${target} POST_BUILD
                COMMAND ${CMAKE_COMMAND} -E copy_if_different "${PNL_LIB_DIRECTORY}/${entry}"  $<TARGET_FILE_DIR:${target}>
                )
        endforeach(entry)
	endif(MSVC)
endfunction(pnl_add_postbuild)


# link_directories(${pnl_root_dir}/lib)
if (PNL_FOUND)
    if (MSVC)
        set(PNL_INCLUDE_DIRS ${PNL_INCLUDE_DIR})
        set(PNL_LIBRARIES ${PNL_LIBRARY})
        add_definitions(-D_ALLOW_KEYWORD_MACROS)
    else (MSVC)
        set(PNL_INCLUDE_DIRS ${PNL_INCLUDE_DIR} )
        set(PNL_LIBRARIES ${PNL_LIBRARY} /usr/local/mingw64/x86_64-w64-mingw32/bin/libblas.dll;/usr/local/mingw64/x86_64-w64-mingw32/bin/liblapack.dll;/usr/local/mingw64/x86_64-w64-mingw32/bin/libblas.dll)
    endif(MSVC)
    message(STATUS "PNL Include: ${PNL_INCLUDE_DIRS}")
    message(STATUS "PNL Libraries: ${PNL_LIBRARIES}")
endif (PNL_FOUND)
mark_as_advanced(PNL_INCLUDE_DIRS PNL_LIBRARIES)